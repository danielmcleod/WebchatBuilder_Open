using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using ININ.IceLib.Connection;
using ININ.IceLib.Interactions;
using ININ.IceLib.People;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebChatBuilderService.Icelib.Availability
{
    public class AgentAvailability
    {
        private static InteractionsManager _interactionsManager;
        private static ICollection<InteractionQueue> _queues;
        private static PeopleManager _peopleManager;
        private static Logging _logging;
        private static DateTime _lastReset;
        private static readonly string _availabilityResetInterval = ConfigurationManager.AppSettings["AvailabilityResetInterval"];
        private static readonly string _availabilityCacheInterval = ConfigurationManager.AppSettings["AvailabilityCacheInterval"];
        private static int _agentsAvailable;
        private static Dictionary<int, int> _availableAgentsByProfileId = new Dictionary<int, int>();
        private static Dictionary<int, DateTime> _lastAvailabilityCheckByProfileId = new Dictionary<int, DateTime>();
        private static bool _isResetting;

        public void Load(Session session)
        {
            _interactionsManager = InteractionsManager.GetInstance(session);
            _peopleManager = PeopleManager.GetInstance(session);
            _queues = new Collection<InteractionQueue>();
            _logging = Logging.GetInstance();
            _lastReset = DateTime.Now;
            _agentsAvailable = -1;
            _isResetting = false;
        }

        public void Unload()
        {
            try
            {
                foreach (var interactionQueue in _queues)
                {
                    if (interactionQueue.IsWatching())
                    {
                        interactionQueue.StopWatching();
                    }
                }
            }
            catch (Exception)
            {
            }
            _queues = null;
            _interactionsManager = null;
        }

        private static int ResetInterval()
        {
            try
            {
                int resetInterval;
                return Int32.TryParse(_availabilityResetInterval, out resetInterval) ? resetInterval : 60;
            }
            catch (Exception)
            {
                
            }
            return 60;
        }

        private static int CacheInterval()
        {
            try
            {
                int cacheInterval;
                return Int32.TryParse(_availabilityCacheInterval, out cacheInterval) ? cacheInterval : 5;
            }
            catch (Exception)
            {

            }

            return 5;
        }


        public static void ResetWatched()
        {
            _lastReset = DateTime.Now;
            _isResetting = true;

            try
            {
                LogNote("Resetting Agent Availability queue watches.");

                foreach (var interactionQueue in _queues)
                {
                    LogNote("Resetting: " + interactionQueue.QueueId.QueueName);

                    if (interactionQueue.IsWatching())
                    {
                        interactionQueue.StopWatching();
                    }
                }
                _queues.Clear();
                LogNote("Agent Availability queue watches reset sucessfully.");
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
            }
            _isResetting = false;
        }

        public static bool ShouldReset()
        {
            var now = DateTime.Now;
            if (_lastReset.AddMinutes(ResetInterval()) < now && !_isResetting)
            {
                LogNote("Agent Availability queue watches should reset.");
                return true;
            }

            return false;
        }

        public static void LogNote(string note)
        {
            try
            {
                _logging.LogNote(note);
            }
            catch (Exception)
            {
            }
        }

        public static bool UseCachedValue(int profileId)
        {
            var now = DateTime.Now;

            if (_availableAgentsByProfileId.ContainsKey(profileId) && _lastAvailabilityCheckByProfileId.ContainsKey(profileId))
            {
                DateTime lastCheck;
                if (_lastAvailabilityCheckByProfileId.TryGetValue(profileId, out lastCheck))
                {
                    return lastCheck.AddSeconds(CacheInterval()) > now;
                }
            }

            return false;
        }

        public static int GetAgentAvailabilityByProfile(int profileId)
        {
            try
            {
                if (UseCachedValue(profileId) || _isResetting)
                {
                    int agentsAvailable;
                    if(_availableAgentsByProfileId.TryGetValue(profileId, out agentsAvailable))
                    {
                        return agentsAvailable;
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
            }

            if (_lastAvailabilityCheckByProfileId.ContainsKey(profileId))
            {
                _lastAvailabilityCheckByProfileId[profileId] = DateTime.Now;
            }
            else
            {
                _lastAvailabilityCheckByProfileId.Add(profileId, DateTime.Now);
            }

            if (ShouldReset())
            {
                ResetWatched();
            }
            var logMessage = "";
            var availableAgents = 0;
            try
            {
                var repository = new Repository();
                var profile = repository.Profiles.FirstOrDefault(p => p.ProfileId == profileId);
                if (profile != null && _peopleManager != null && _peopleManager.Session != null && _peopleManager.Session.ConnectionState == ConnectionState.Up)
                {
                    var workgroup = profile.Workgroup;
                    if (workgroup != null)
                    {
                        logMessage += "/~ Checking Availability for: " + workgroup.ConfigId + "~/";

                        var wgUtils = workgroup.Utilizations.ToList();
                        var agents = workgroup.ActiveMembers;
                        foreach (var agent in agents)
                        {
                            try
                            {
                                logMessage += "/~ Checking Agent: " + agent.ConfigId + "~/";

                                var usl = new UserStatusList(_peopleManager);
                                var status = usl.GetUserStatus(agent.ConfigId);
                                var hasRequiredSkills = true;
                                var acdAvailable = status.LoggedIn && status.StatusMessageDetails.IsAcdStatus;

                                logMessage += "/~ Status: " + status.StatusMessageDetails.MessageText + "~/";
                                logMessage += "/~ Logged In: " + status.LoggedIn + "~/";
                                logMessage += "/~ Is Stale: " + status.IsStale + "~/";
                                logMessage += "/~ Station Count: " + status.Stations.Count + "~/";

                                var agentSkills = agent.Skills;
                                foreach (var skill in profile.Skills)
                                {
                                    if (!agentSkills.Contains(skill))
                                    {
                                        hasRequiredSkills = false;
                                    }
                                }
                                logMessage += "/~ Has Required Skills: " + hasRequiredSkills + "~/";
                                var agentUtils = agent.Utilizations.ToList();
                                var utils = agentUtils.Any(u => u.MediaType == MediaType.Chat) ? agentUtils : wgUtils;
                                var maxChats = 1;
                                if (utils != null)
                                {
                                    maxChats = utils.FirstOrDefault(m => m.MediaType == MediaType.Chat).MaxAssignable;
                                }

                                if ((agent.IsLicensedForChat || agent.MediaLevel == 3) && agent.HasActiveClientLicense &&
                                    //agent.ActiveInWorkgroups.Contains(workgroup) && //already finding agents for workgroup above
                                    hasRequiredSkills && utils.Any(u => u.MediaType == MediaType.Chat) && acdAvailable && maxChats > 0)
                                {
                                    logMessage += "/~ Meets base criteria ~/";

                                    var watched = _queues.FirstOrDefault(q => q.QueueId.QueueName == agent.ConfigId && q.QueueId.QueueType == QueueType.User);
                                    var maxReached = false;
                                    if (((watched != null && !watched.IsWatching()) || watched == null) && _interactionsManager != null)
                                    {
                                        if (watched != null)
                                        {
                                            _queues.Remove(watched);
                                        }
                                        var interactionQueue = new InteractionQueue(_interactionsManager,
                                            new QueueId(QueueType.User, agent.ConfigId));
                                        string[] attributes =
                                        {
                                            InteractionAttributeName.InteractionType,
                                            InteractionAttributeName.State,
                                            InteractionAttributeName.Direction
                                        };
                                        interactionQueue.StartWatching(attributes);
                                        _queues.Add(interactionQueue);
                                        watched = interactionQueue;
                                    }
                                    var interactions = watched.GetContents();

                                    var callUtil = agentUtils.Any(u => u.MediaType == MediaType.Call) ? agentUtils.FirstOrDefault(u => u.MediaType == MediaType.Call).UtilizationPercent : wgUtils.Any(u => u.MediaType == MediaType.Call) ? wgUtils.FirstOrDefault(u => u.MediaType == MediaType.Call).UtilizationPercent : 0;
                                    var callbackUtil = agentUtils.Any(u => u.MediaType == MediaType.Callback) ? agentUtils.FirstOrDefault(u => u.MediaType == MediaType.Callback).UtilizationPercent : wgUtils.Any(u => u.MediaType == MediaType.Callback) ? wgUtils.FirstOrDefault(u => u.MediaType == MediaType.Callback).UtilizationPercent : 0;
                                    var chatUtil = agentUtils.Any(u => u.MediaType == MediaType.Chat) ? agentUtils.FirstOrDefault(u => u.MediaType == MediaType.Chat).UtilizationPercent : wgUtils.Any(u => u.MediaType == MediaType.Chat) ? wgUtils.FirstOrDefault(u => u.MediaType == MediaType.Chat).UtilizationPercent : 0;
                                    var emailUtil = agentUtils.Any(u => u.MediaType == MediaType.Email) ? agentUtils.FirstOrDefault(u => u.MediaType == MediaType.Email).UtilizationPercent : wgUtils.Any(u => u.MediaType == MediaType.Email) ? wgUtils.FirstOrDefault(u => u.MediaType == MediaType.Email).UtilizationPercent : 0;
                                    var genericUtil = agentUtils.Any(u => u.MediaType == MediaType.Generic) ? agentUtils.FirstOrDefault(u => u.MediaType == MediaType.Generic).UtilizationPercent : wgUtils.Any(u => u.MediaType == MediaType.Generic) ? wgUtils.FirstOrDefault(u => u.MediaType == MediaType.Generic).UtilizationPercent : 0;
                                    var workitemUtil = agentUtils.Any(u => u.MediaType == MediaType.WorkItem) ? agentUtils.FirstOrDefault(u => u.MediaType == MediaType.WorkItem).UtilizationPercent : wgUtils.Any(u => u.MediaType == MediaType.WorkItem) ? wgUtils.FirstOrDefault(u => u.MediaType == MediaType.WorkItem).UtilizationPercent : 0;

                                    var util = chatUtil;
                                    foreach (var interaction in interactions)
                                    {
                                        //Should I be checking for WorkgroupQueue or other identifier in case a non ACD call is connected and the agent is technically unavailable?
                                        var type = interaction.InteractionType;
                                        var isActive = (interaction.State != InteractionState.ExternalDisconnect &&
                                                        interaction.State != InteractionState.InternalDisconnect &&
                                                        interaction.State != InteractionState.None);
                                        if (type == InteractionType.Chat && isActive)
                                        {
                                            util += chatUtil;
                                            maxChats--;
                                        }
                                        if (type == InteractionType.Call && isActive)
                                        {
                                            util += callUtil;
                                        }
                                        if (type == InteractionType.Callback && isActive)
                                        {
                                            util += callbackUtil;
                                        }
                                        if (type == InteractionType.Email && isActive)
                                        {
                                            util += emailUtil;
                                        }
                                        if (type == InteractionType.Generic && isActive)
                                        {
                                            util += genericUtil;
                                        }
                                        if (type == InteractionType.WorkItem && isActive)
                                        {
                                            util += workitemUtil;
                                        }
                                        if (maxChats < 1 || util > 100)
                                        {
                                            maxReached = true;
                                        }
                                    }
                                    if (!maxReached)
                                    {
                                        logMessage += "/~ Agent " + agent.ConfigId + " is available. ~/";
                                        availableAgents++;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                logMessage += "/~" + e.ToString() + "~/";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logMessage += "/~" + e.ToString() + "~/";
            }

            logMessage += "/~ Agents Available: " + availableAgents + "~/";

            try
            {
                _logging.LogNote(logMessage);
            }
            catch (Exception)
            {
            }

            if (_availableAgentsByProfileId.ContainsKey(profileId))
            {
                _availableAgentsByProfileId[profileId] = availableAgents;
            }
            else
            {
                _availableAgentsByProfileId.Add(profileId, availableAgents);                
            }
            return availableAgents;
        }
    }
}
