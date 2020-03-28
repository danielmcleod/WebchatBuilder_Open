using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.DataTypes;
using ININ.IceLib.Connection;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;
using WebChatBuilderService.Icelib.Interactions;
using WebChatBuilderService.Icelib.People;
using MediaType = ININ.IceLib.Configuration.DataTypes.MediaType;

namespace WebChatBuilderService.Icelib.Configurations
{
    public class AgentConfigurations
    {
        private ConfigurationManager _configurationManager;
        private Logging _logging;

        private UserConfigurationList _userConfigurationList;

        private ICollection<UserConfiguration> _users = new Collection<UserConfiguration>();

        private static bool _firstRunAfterStart = true;
        private static bool _skillsChanged = false;

        protected Repository Repository
        {
            get { return new Repository(); }
        }

        public void Load(Session session)
        {
            _configurationManager = ConfigurationManager.GetInstance(session);
            _logging = Logging.GetInstance();
            StartWatchingUsers();
        }

        public void Unload()
        {
            try
            {
                if (_userConfigurationList.IsWatching)
                {
                    _userConfigurationList.StopWatching();
                }
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            _userConfigurationList = null;
            _configurationManager = null;
        }

        public void RefreshWatches()
        {
            if (_userConfigurationList.IsWatching)
            {
                _userConfigurationList.RefreshWatchAsync(Users_StartOrRefreshWatchingCompleted, null);
            }
        }

        private void StartWatchingUsers()
        {
            //_logging.LogNote("StartWatchingUsers");
            try
            {
                if (_configurationManager != null && _configurationManager.Session != null && _configurationManager.Session.ConnectionState == ConnectionState.Up)
                {
                    _userConfigurationList = new UserConfigurationList(_configurationManager);
                    var querySettings = _userConfigurationList.CreateQuerySettings();
                    querySettings.SetResultCountLimit(QueryResultLimit.Unlimited);

                    var properties = new List<UserConfiguration.Property>
                    {
                        UserConfiguration.Property.Id,
                        UserConfiguration.Property.DisplayName,
                        UserConfiguration.Property.Utilizations,
                        UserConfiguration.Property.Skills,
                        UserConfiguration.Property.License_LicenseActive,
                        UserConfiguration.Property.License_HasClientAccess,
                        UserConfiguration.Property.License_MediaLevel,
                        UserConfiguration.Property.License_MediaTypes,
                        UserConfiguration.Property.Workgroups
                    };

                    querySettings.SetPropertiesToRetrieve(properties);
                    using (var repository = Repository)
                    {
                        if (!repository.Agents.Any())
                        {
                            _userConfigurationList.StartCaching(querySettings);
                            var users = _userConfigurationList.GetConfigurationList();
                            _userConfigurationList.StopCaching();
                            if (users.Any())
                            {
                                foreach (var userConfiguration in users)
                                {
                                    CreateAgent(userConfiguration);
                                }
                                _users = users;
                            }
                        }
                    }

                    _userConfigurationList.ConfigurationObjectsRemoved += Users_ConfigurationObjectsRemoved;

                    _userConfigurationList.ConfigurationObjectsChanged += Users_ConfigurationObjectsChanged;

                    _userConfigurationList.StartWatchingAsync(querySettings, Users_StartOrRefreshWatchingCompleted, null);
                }
            }
            catch (Exception e)
            {
                _logging.LogException(e);
                _logging.TraceException(e, "AgentConfigurations Error");
            }
        }

        private void Users_ConfigurationObjectsChanged(object sender, ConfigurationObjectChangesEventArgs<UserConfiguration, UserConfiguration.Property> e)
        {
            try
            {
                //_logging.LogNote("Users_ConfigurationObjectsChanged");
                foreach (var changedObject in e.ObjectsChanged)
                {
                    var updated = false;
                    var userConfiguration = changedObject.Key;

                    using (var repository = Repository)
                    {
                        var agent = repository.Agents.FirstOrDefault(a => a.ConfigId == userConfiguration.ConfigurationId.Id);
                        if (agent != null)
                        {
                            //_logging.LogNote("Agent: " + agent.DisplayName);
                            if (changedObject.Value.Contains(UserConfiguration.Property.Skills))
                            {
                                //_logging.LogNote("Skills Changed");
                                foreach (var skill in agent.Skills.ToList())
                                {
                                    agent.Skills.Remove(skill);
                                }
                                var skills = userConfiguration.Skills.Value.Select(s => s.Id.Id);
                                var inheritedSkills = userConfiguration.Skills.InheritedValue.Select(s => s.Id.Id);

                                //in addition to value, i also need inherited value
                                var skls =
                                    skills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                        .Where(sk => sk != null)
                                        .ToList();
                                if (inheritedSkills.Any())
                                {
                                    skls.AddRange(inheritedSkills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                        .Where(sk => sk != null)
                                        .ToList());
                                }

                                agent.Skills = skls;
                                updated = true;
                                //var skillString = String.Join(",", skls.Select(i => i.DisplayName));
                                //_logging.LogNote(String.Format("Skills: {0}", skillString));
                            }
                            if (changedObject.Value.Contains(UserConfiguration.Property.Utilizations))
                            {
                                //_logging.LogNote("Utilizations Changed");

                                var utilList = agent.Utilizations.ToList();
                                agent.Utilizations.Clear();
                                foreach (var utilization in utilList)
                                {
                                    repository.Utilizations.Remove(utilization);
                                }

                                var utilizations = userConfiguration.Utilizations;
                                var utils = utilizations.Value;
                                var u = utils.Select(utilizationSettings => new Utilization
                                {
                                    MediaType = GetMediaType(utilizationSettings.MediaType),
                                    MaxAssignable = utilizationSettings.MaxAssignable,
                                    UtilizationPercent = utilizationSettings.Utilization
                                }).ToList();
                                repository.Utilizations.AddRange(u);
                                agent.Utilizations = u;
                                updated = true;
                                //var utilizationString = String.Join(",", u.Select(i => i.MediaType));
                                //_logging.LogNote(String.Format("Utilizations: {0}", utilizationString));
                                
                            }
                            if (changedObject.Value.Contains(UserConfiguration.Property.Workgroups))
                            {
                                try
                                {
                                    var workgroups = userConfiguration.Workgroups;
                                    var wgs = workgroups.Value;
                                    foreach (var wg in wgs.Select(i => i.Id))
                                    {
                                        if (agent.ActiveInWorkgroups.Select(i => i.ConfigId).Any(i => i == wg))
                                        {
                                            WorkgroupPeople.RefreshWatch = true;
                                            WorkgroupInteractions.RefreshWatch = true;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logging.LogException(ex);
                                    _logging.TraceException(ex, "AgentConfigurations Error - Workgroups Changed");
                                }
                            }
                            if (updated)
                            {
                                repository.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "AgentConfigurations Error");
            }
        }

        private void Users_StartOrRefreshWatchingCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var userConfigurationList = sender as UserConfigurationList;
            ProcessUserConfigurations(userConfigurationList);
        }

        private static readonly object ProcessingLock = new object();

        private void ProcessUserConfigurations(UserConfigurationList userConfigurationList)
        {
            //_logging.LogNote("ProcessUserConfigurations");
            lock (ProcessingLock)
            try
            {
                if (userConfigurationList != null)
                {
                    var users = userConfigurationList.GetConfigurationList();
                    if (!_users.Any() && users.Any())
                    {
                        foreach (var userConfiguration in users)
                        {
                            CreateAgent(userConfiguration);
                        }
                        _users = users;
                    }
                    else if (UsersChanged(users))
                    {
                        foreach (var userConfiguration in users)
                        {
                            CreateAgent(userConfiguration);
                        }
                        _skillsChanged = false;
                        _users = users;
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "AgentConfigurations Error");
            }
            _firstRunAfterStart = false;
        }

        private void CreateAgent(UserConfiguration userConfiguration)
        {
            //_logging.LogNote("CreateAgent");
            try
            {
                using (var repository = Repository)
                {
                    var exist = repository.Agents.Any(a => a.ConfigId == userConfiguration.ConfigurationId.Id);
                    if (!exist)
                    {
                        //_logging.LogNote("No Agents Exist");
                        var utilizations = userConfiguration.Utilizations;
                        var skills = userConfiguration.Skills.Value.Select(s => s.Id.Id);
                        var inheritedSkills = userConfiguration.Skills.InheritedValue.Select(s => s.Id.Id);

                        //Licensing
                        var hasClientAccess = userConfiguration.License.HasClientAccess.Value &&
                                              userConfiguration.License.LicenseActive.Value;
                        var mediaLevel = userConfiguration.License.MediaLevel.Value;
                        var mediaTypes = userConfiguration.License.MediaTypes.Value;

                        var utils = utilizations.Value;

                        var u = utils.Select(utilizationSettings => new Utilization
                        {
                            MediaType = GetMediaType(utilizationSettings.MediaType),
                            MaxAssignable = utilizationSettings.MaxAssignable,
                            UtilizationPercent = utilizationSettings.Utilization
                        }).ToList();
                        repository.Utilizations.AddRange(u);

                        var skls =
                            skills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                .Where(sk => sk != null)
                                .ToList();

                        if (inheritedSkills.Any())
                        {
                            skls.AddRange(inheritedSkills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                .Where(sk => sk != null)
                                .ToList());
                        }

                        var agent = new Agent
                        {
                            DisplayName = userConfiguration.ConfigurationId.DisplayName,
                            ConfigId = userConfiguration.ConfigurationId.Id,
                            HasActiveClientLicense = hasClientAccess,
                            MediaLevel =
                                mediaLevel == MediaLevel.Media1
                                    ? 1
                                    : mediaLevel == MediaLevel.Media2 ? 2 : mediaLevel == MediaLevel.Media3 ? 3 : 0,
                            IsLicensedForChat = mediaTypes.Contains(MediaType.Chat),
                            Skills = skls,
                            Utilizations = u,
                        };
                        //var skillString = String.Join(",", skls.Select(i => i.DisplayName));
                        //var utilizationString = String.Join(",", u.Select(i => i.MediaType));

                        //_logging.LogNote(String.Format("Agent Name: {0}, Client License {1}, Media Level {2}, Chat License: {3}, Skills: {4}, Utilizations: {5}", agent.DisplayName,agent.HasActiveClientLicense,agent.MediaLevel,agent.IsLicensedForChat,skillString,utilizationString));
                        repository.Agents.Add(agent);
                        repository.SaveChanges();
                        if (userConfiguration.Workgroups != null && userConfiguration.Workgroups.Value != null && userConfiguration.Workgroups.Value.Any())
                        {
                            WorkgroupPeople.RefreshWatch = true;
                            WorkgroupInteractions.RefreshWatch = true;
                        }
                    }
                    else
                    {
                        if (_firstRunAfterStart)
                        {
                            //_logging.LogNote("First Run After Start");
                            var agent = repository.Agents.FirstOrDefault(a => a.ConfigId == userConfiguration.ConfigurationId.Id);
                            if (agent != null)
                            {
                                var utilizations = userConfiguration.Utilizations;
                                //var refList = String.Join(",", utilizations.InheritedValueDetails.ReferenceList);
                                var val = String.Join(",", utilizations.Value.Select(i => "MediaType:" + i.MediaType + " Percent:" + i.Utilization + " Max:" + i.MaxAssignable));
                                //var effective = String.Join(",", utilizations.EffectiveValue.Select(i => "MediaType:" + i.MediaType + " Percent:" + i.Utilization + " Max:" + i.MaxAssignable));
                                //var inherited = String.Join(",", utilizations.InheritedValue.Select(i => "MediaType:" + i.MediaType + " Percent:" + i.Utilization + " Max:" + i.MaxAssignable));

                                ////_logging.LogNote("ReferenceList: " + refList);
                                //_logging.LogNote("Value: " + val);
                                ////_logging.LogNote("Effective: " + effective);
                                ////_logging.LogNote("Inherited: " + inherited);
                                var skills = userConfiguration.Skills.Value.Select(s => s.Id.Id);
                                var inheritedSkills = userConfiguration.Skills.InheritedValue.Select(s => s.Id.Id);

                                //Licensing
                                var hasClientAccess = userConfiguration.License.HasClientAccess.Value &&
                                                      userConfiguration.License.LicenseActive.Value;
                                var mediaLevel = userConfiguration.License.MediaLevel.Value;
                                var mediaTypes = userConfiguration.License.MediaTypes.Value;

                                var utils = utilizations.Value;

                                var u = utils.Select(utilizationSettings => new Utilization
                                {
                                    MediaType = GetMediaType(utilizationSettings.MediaType),
                                    MaxAssignable = utilizationSettings.MaxAssignable,
                                    UtilizationPercent = utilizationSettings.Utilization
                                }).ToList();
                                repository.Utilizations.AddRange(u);

                                var skls =
                                    skills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                        .Where(sk => sk != null)
                                        .ToList();
                                if (inheritedSkills.Any())
                                {
                                    skls.AddRange(inheritedSkills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                        .Where(sk => sk != null)
                                        .ToList());
                                }

                                agent.DisplayName = userConfiguration.ConfigurationId.DisplayName;
                                agent.ConfigId = userConfiguration.ConfigurationId.Id;
                                agent.HasActiveClientLicense = hasClientAccess;
                                agent.MediaLevel = mediaLevel == MediaLevel.Media1
                                    ? 1
                                    : mediaLevel == MediaLevel.Media2 ? 2 : mediaLevel == MediaLevel.Media3 ? 3 : 0;
                                agent.IsLicensedForChat = mediaTypes.Contains(MediaType.Chat);
                                agent.Skills.Clear();
                                var utilList = agent.Utilizations.ToList();
                                agent.Utilizations.Clear();
                                foreach (var utilization in utilList)
                                {
                                    repository.Utilizations.Remove(utilization);
                                }
                                agent.Utilizations = u;
                                //repository.SaveChanges();
                                agent.Skills = skls;
                                //var skillString = String.Join(",", skls.Select(i => i.DisplayName));
                                //var utilizationString = String.Join(",", u.Select(i => i.MediaType));

                                //_logging.LogNote(String.Format("Agent Name: {0}, Client License {1}, Media Level {2}, Chat License: {3}, Skills: {4}, Utilizations: {5}", agent.DisplayName, agent.HasActiveClientLicense, agent.MediaLevel, agent.IsLicensedForChat, skillString, utilizationString));
                                repository.SaveChanges();
                            }
                        }
                        else if (_skillsChanged)
                        {
                            var agent = repository.Agents.FirstOrDefault(a => a.ConfigId == userConfiguration.ConfigurationId.Id);
                            if (agent != null)
                            {
                                var skills = userConfiguration.Skills.Value.Select(s => s.Id.Id);
                                var inheritedSkills = userConfiguration.Skills.InheritedValue.Select(s => s.Id.Id);
                                var skls =
                                    skills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                        .Where(sk => sk != null)
                                        .ToList();
                                if (inheritedSkills.Any())
                                {
                                    skls.AddRange(inheritedSkills.Select(skill => repository.Skills.FirstOrDefault(s => s.ConfigId == skill))
                                        .Where(sk => sk != null)
                                        .ToList());
                                }
                                agent.Skills.Clear();
                                agent.Skills = skls;
                                repository.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logging.LogException(e);
                _logging.TraceException(e, "AgentConfigurations Error");
            }
        }

        private bool UsersChanged(ICollection<UserConfiguration> users)
        {
            if (users.Count != _users.Count)
            {
                return true;
            }

            var skills = users.Select(u => u.Skills.Value).ToList();
            var inheritedSkills = users.Select(u => u.Skills.InheritedValue).ToList();

            var existingSkills = _users.Select(u => u.Skills.Value).ToList();
            var existingInheritedSkills = _users.Select(u => u.Skills.InheritedValue).ToList();

            if (skills.Count != existingSkills.Count || inheritedSkills.Count != existingInheritedSkills.Count)
            {
                _skillsChanged = true;
                return true;
            }

            var workgroups = users.Select(u => u.Workgroups.Value).ToList();

            var existingWorkgroups = _users.Select(u => u.Workgroups.Value).ToList();

            if (workgroups.Count != existingWorkgroups.Count)
            {
                WorkgroupPeople.RefreshWatch = true;
                WorkgroupInteractions.RefreshWatch = true;
                return true;
            }

            try
            {
                var s1 = new HashSet<object>(skills);
                var s2 = new HashSet<object>(existingSkills);
                var areSkillsEqual = s1.SetEquals(s2);
                //_logging.LogNote("Are Skills Equal?: " + areSkillsEqual);

                var i1 = new HashSet<object>(inheritedSkills);
                var i2 = new HashSet<object>(existingInheritedSkills);
                var areInheritedEqual = i1.SetEquals(i2);
                //_logging.LogNote("Are Inherited Skills Equal?: " + areInheritedEqual);
                if (!areSkillsEqual || !areInheritedEqual)
                {
                    _skillsChanged = true;
                    return true;
                }


                var w1 = new HashSet<object>(skills);
                var w2 = new HashSet<object>(existingSkills);
                var areWorkgroupsEqual = w1.SetEquals(w2);
                if (!areWorkgroupsEqual)
                {
                    WorkgroupPeople.RefreshWatch = true;
                    WorkgroupInteractions.RefreshWatch = true;
                    return true;
                }
            }
            catch (Exception e)
            {
                _logging.LogException(e);
                _logging.TraceException(e, "AgentConfigurations Error");
            }

            var userIds = users.Select(u => u.ConfigurationId.Id);
            return userIds.Any(userId => !_users.Select(u => u.ConfigurationId.Id).Contains(userId));
        }

        private void Users_ConfigurationObjectsRemoved(object sender, ConfigurationWatchEventArgs<UserConfiguration> e)
        {
            //_logging.LogNote("Users_ConfigurationObjectsRemoved");
            try
            {
                var removeUsers = e.ObjectsAffected;
                using (var repository = Repository)
                {
                    foreach (var user in removeUsers.Select(userConfiguration => repository.Agents.FirstOrDefault(a => a.ConfigId == userConfiguration.ConfigurationId.Id)).Where(user => user != null))
                    {
                        foreach (var utilization in user.Utilizations.ToList())
                        {
                            //user.Utilizations.Remove(utilization);
                            repository.Utilizations.Remove(utilization);
                        }
                        if (user.ActiveInWorkgroups != null)
                        {
                            foreach (var workgroup in user.ActiveInWorkgroups.ToList())
                            {
                                user.ActiveInWorkgroups.Remove(workgroup);
                            }
                        }
                        user.Skills.Clear();
                        user.Utilizations.Clear();
                        //foreach (var skill in user.Skills.ToList())
                        //{
                        //    user.Skills.Remove(skill);
                        //}
                        repository.Agents.Remove(user);
                        repository.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "AgentConfigurations Error");                
            }
        }

        private WebChatBuilderModels.Models.MediaType GetMediaType(MediaType mediaType)
        {
            if (mediaType == MediaType.Call)
            {
                return WebChatBuilderModels.Models.MediaType.Call;
            }
            if (mediaType == MediaType.Email)
            {
                return WebChatBuilderModels.Models.MediaType.Email;
            }
            if (mediaType == MediaType.Chat)
            {
                return WebChatBuilderModels.Models.MediaType.Chat;
            }
            if (mediaType == MediaType.Callback)
            {
                return WebChatBuilderModels.Models.MediaType.Callback;
            }
            if (mediaType == MediaType.Generic)
            {
                return WebChatBuilderModels.Models.MediaType.Generic;
            }
            if (mediaType == MediaType.WorkItem)
            {
                return WebChatBuilderModels.Models.MediaType.WorkItem;
            }
            return WebChatBuilderModels.Models.MediaType.Invalid;
        }
    }
}
