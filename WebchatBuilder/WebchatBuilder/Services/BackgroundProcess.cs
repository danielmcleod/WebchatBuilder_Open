using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using WebchatBuilder.Controllers;
using WebchatBuilder.DataModels;
using WebchatBuilder.Helpers;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.Services
{
    public class BackgroundProcess : IRegisteredObject
    {
        private Timer TaskTimer;
        private bool _running;
        private int _checkCicUrl = 0;
        private int _checkLicense = 0;
        private int _checkFiles = 3600;
        private int _refreshWorkgroupWatches = 3600;
        private int _checkDisconnects = 3600;
        private int _checkQueuedMessages = 1800;
        private int _checkVisitorMessages = 300;
        private int _checkSchedules = 300;
        private int _checkParty = 0;
        private static bool _processingVisitorMessages = false;

        public BackgroundProcess()
        {
            HostingEnvironment.RegisterObject(this);
            _running = true;
            ChatServices.Licensed = ChatServices.IsLicensed();
            var isLicensed = ChatServices.Licensed;
            _checkLicense = isLicensed ? 1800 : 5;
            var baseUrl = ChatServices.BaseUrl;
            TaskTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
            //using (var db = new Repository())
            //{
            //    var load = db.Profiles.FirstOrDefault();
            //} 
        }

        private void OnTimerElapsed(object sender)
        {
            try
            {
                if (_checkLicense == 0)
                {
                    ChatServices.Licensed = ChatServices.IsLicensed();
                    var isLicensed = ChatServices.Licensed;
                    _checkLicense = isLicensed ? 1800 : 5;
                    if (!isLicensed)
                    {
                        ChatServices.AddAlert("Wcb Unlicensed", "Wcb License not found or License is Invalid.", 9999);
                    }
                }
                else
                {
                    _checkLicense = _checkLicense - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                _checkParty++;
                if (ChatServices.WebChats.Any() && !ChatServices.Reconnecting)
                {
                    var wasChecked = false;
                    foreach (var webChat in ChatServices.WebChats.Where(w => !String.IsNullOrWhiteSpace(w.ConnectionId) && !String.IsNullOrWhiteSpace(w.ParticipantId)))
                    {
                        var wc = webChat;
                        if (_checkParty > 1 &&!String.IsNullOrWhiteSpace(wc.AgentParticipantId) && wc.State == ChatState.Connected)
                        {
                            wasChecked = true;
                            Task.Run(() => CheckPartyInfo(wc.ChatId, wc.ParticipantId, wc.AgentParticipantId));
                        }
                        Task.Run(() => PollChat(wc));
                    }
                    if (wasChecked)
                    {
                        _checkParty = 0;
                    }
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (ChatServices.VisitorMessageChats.Any() && !ChatServices.Reconnecting)
                {
                    foreach (var visitorMessage in ChatServices.VisitorMessageChats)
                    {
                        var vm = visitorMessage;
                        Task.Run(() => PollVisitorMessage(vm));
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }

            try
            {
                if (_checkCicUrl == 0)
                {
                    _checkCicUrl = 5;
                    ChatServices.CicServer();
                }
                else
                {
                    _checkCicUrl = _checkCicUrl - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (_checkFiles == 0)
                {
                    _checkFiles = 3600;
                    Task.Run(() => ProcessFiles());
                }
                else
                {
                    _checkFiles = _checkFiles - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (_checkDisconnects == 0)
                {
                    _checkDisconnects = 3600;
                    ProcessDisconnects();
                }
                else
                {
                    _checkDisconnects = _checkDisconnects - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (_checkQueuedMessages == 0)
                {
                    _checkQueuedMessages = 1800;
                    ProcessQueuedMessages();
                }
                else
                {
                    _checkQueuedMessages = _checkQueuedMessages - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (_checkVisitorMessages == 0)
                {
                    _checkVisitorMessages = 300;
                    Task.Run(() => ProcessVisitorMessages());
                }
                else
                {
                    _checkVisitorMessages = _checkVisitorMessages - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (_checkSchedules == 0)
                {
                    _checkSchedules = 300;
                    CheckForScheduleUpdates();
                }
                else
                {
                    _checkSchedules = _checkSchedules - 1;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (_refreshWorkgroupWatches == 0)
                {
                    ChatServices.UpdateRefreshWatch();
                    _refreshWorkgroupWatches = 3600;
                }
                else
                {
                    _refreshWorkgroupWatches = _refreshWorkgroupWatches - 1;
                }
            }
            catch (Exception)
            {
            }
        }

        void CheckForScheduleUpdates()
        {
            try
            {
                if (!ScheduleManager.LastUpdatedUtc.HasValue)
                {
                    //new Task(ScheduleManager.LoadAllSchedules).Start();
                    Task.Run(() => ScheduleManager.LoadAllSchedules());
                }
                else
                {
                    var lastServerUpdate = ChatServices.LastScheduleUpdate();
                    var hasServerUpdates = lastServerUpdate.HasValue && lastServerUpdate.Value > ScheduleManager.LastUpdatedUtc.Value;
                    var newDateToProcess = ScheduleManager.LastUpdatedUtc.Value.AddHours(ScheduleManager.ScheduleDateTimeOffset).Date < DateTime.UtcNow.AddHours(ScheduleManager.ScheduleDateTimeOffset).Date;
                    if (hasServerUpdates|| newDateToProcess)
                    {
                        //new Task(ScheduleManager.LoadAllSchedules).Start();
                        Task.Run(() => ScheduleManager.LoadAllSchedules());
                    }
                    else
                    {
                        if (ScheduleManager.ProfilesToReload.Any())
                        {
                            Task.Run(() => ScheduleManager.UpdateChangedScheduleAvailability());
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        void ProcessVisitorMessages()
        {
            try
            {
                if (_processingVisitorMessages)
                {
                    return;
                }
                _processingVisitorMessages = true;
                var useCallback = ChatController.UseCallbackForVMs();
                var useWorkgroupFromProfile = ChatController.UseProfileWorkgroupForVMs();
                var vmWorkgroup = ChatController.GetDefaultWorkgroupForVisitorMessages();
                var repository = new Repository();
                var targetWorkgroup = repository.Workgroups.FirstOrDefault(w => w.DisplayName.ToLower() == vmWorkgroup);
                if (repository.VisitorMessages.Any(v => !v.IsProcessed) && (targetWorkgroup != null || useWorkgroupFromProfile))
                {
                    //LoggingService.GetInstance().LogToFile("Visitor Messages Found", ChatServices.FolderNameSafeDate(), true);
                    var visitorMessages = repository.VisitorMessages.Where(v => !v.IsProcessed).ToList();
                    var chatAttributes = new Dictionary<string, string>();
                    var chatSkills = new Collection<RoutingContext>();
                    
                    foreach (var visitorMessage in visitorMessages)
                    {
                        if (!useCallback)
                        {
                            if (!ChatServices.VisitorMessageChats.Any(i => i.VisitorMessageId == visitorMessage.VisitorMessageId))
                            {
                                var chatRequest = new ChatRequest
                                {
                                    SupportedContentTypes = "text/plain",
                                    Participant = new Participant
                                    {
                                        Name = "visitormessage",
                                        Credentials = ""
                                    },
                                    TranscriptRequired = false,
                                    EmailAddress = "unknown@unknown.com",
                                    Target = useWorkgroupFromProfile ? visitorMessage.Workgroup : targetWorkgroup.DisplayName,
                                    TargetType = "Workgroup",
                                    Language = "en-us",
                                    CustomInfo = "",
                                    Attributes = chatAttributes,
                                    RoutingContexts = chatSkills
                                };
                                var chatResponse = ChatServices.StartChat(chatRequest);
                                if (chatResponse != null && chatResponse.Status.Type == "success")
                                {
                                    var vm = new VisitorMessageChat
                                    {
                                        VisitorMessageId = visitorMessage.VisitorMessageId,
                                        Message = visitorMessage.Message,
                                        ParticipantId = chatResponse.ParticipantID,
                                        State = VisitorMessageState.Connected
                                    };
                                    ChatServices.VisitorMessageChats.Add(vm);
                                }
                            }
                        }
                        else
                        {
                            var callbackAttributes = new Dictionary<string, string>();
                            if (!String.IsNullOrWhiteSpace(visitorMessage.AttributeNames))
                            {
                                var attrNames = visitorMessage.AttributeNames.Split('|');
                                var attrValues = visitorMessage.AttributeValues.Split('|');
                                if (attrNames.Length == attrValues.Length)
                                {
                                    for (int i = 0; i < attrNames.Length; i++)
                                    {
                                        var name = attrNames[i];
                                        var val = attrValues[i];
                                        callbackAttributes.Add(name,val);
                                    }
                                }
                            }

                            var callbackSkills = new Collection<CallbackRoutingContext>();
                            if (!String.IsNullOrWhiteSpace(visitorMessage.Skills))
                            {
                                var skills = visitorMessage.Skills.Split(',');
                                foreach (var skill in skills)
                                {
                                    callbackSkills.Add(new CallbackRoutingContext
                                    {
                                        Category = "Product",
                                        Context = skill
                                    });
                                }
                            }

                            var subject = visitorMessage.Message.Length > 2000 ? visitorMessage.Message.Substring(0, 1999) : visitorMessage.Message;

                            var callbackRequest = new CallbackRequest()
                            {
                                Participant = new CallbackParticipant
                                {
                                    Name = "visitormessage",
                                    Credentials = "",
                                    Telephone = visitorMessage.PhoneNumber
                                },
                                Target = useWorkgroupFromProfile ? visitorMessage.Workgroup : targetWorkgroup.DisplayName,
                                TargetType = "Workgroup",
                                Language = "en-us",
                                CustomInfo = visitorMessage.CustomInfo,
                                Subject = subject,
                                Attributes = callbackAttributes,
                                RoutingContexts = callbackSkills
                            };
                            var callbackResponse = ChatServices.CreateCallback(callbackRequest);
                            if (callbackResponse != null && callbackResponse.Status.Type == "success")
                            {
                                visitorMessage.IsProcessed = true;
                                visitorMessage.Type = VisitorMessageType.Callback;
                                repository.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            _processingVisitorMessages = false;
        }

        void ProcessFiles()
        {
            try
            {
                var path = HostingEnvironment.MapPath("~/Files");
                if (path != null)
                {
                    var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                    foreach (var directory in directories)
                    {
                        if (!ChatServices.WebChats.Any(c => c.SessionId == directory))
                        {
                            var dirPath = Path.Combine(path, directory);
                            var downloadedMessageInfo = new DirectoryInfo(dirPath);
                            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                            {
                                file.Delete();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        void ProcessDisconnects()
        {
            if (ChatServices.DisconnectedWebChats.Any())
            {
                foreach (var chatId in ChatServices.DisconnectedWebChats.Where(c => c.DateCreated.HasValue && DateTime.Now.Subtract(c.DateCreated.Value).TotalHours > 12).Select(i => i.ChatId).ToList())
                {
                    var chat = ChatServices.DisconnectedWebChats.FirstOrDefault(c => c.ChatId == chatId);
                    if (chat != null)
                    {
                        ChatServices.DisconnectedWebChats.Remove(chat);
                    }
                }
            }
        }

        void ProcessQueuedMessages()
        {
            if (ChatServices.QueuedMessages.Any())
            {
                foreach (var id in ChatServices.QueuedMessages.Where(m => DateTime.Now.Subtract(m.DateSent).TotalHours > 1).Select(i => i.Id).ToList())
                {
                    var msg = ChatServices.QueuedMessages.FirstOrDefault(m => m.Id == id);
                    if (msg != null)
                    {
                        ChatServices.QueuedMessages.Remove(msg);
                    }
                }
            }
        }

        void PollChat(WebChat webChat)
        {
            if (webChat.State == ChatState.Disconnected)
            {
                Disconnect(webChat, ChatServices.DisconnectReason.UserDisconnect);
                return;
            }
            if (webChat.State == ChatState.Paused || (ChatController.KeepQueuedChatsAlive && webChat.State == ChatState.Queued && webChat.IsMinimized))
            {
                if (webChat.DatePaused.HasValue)
                {
                    var elapsed = DateTime.Now.Subtract(webChat.DatePaused.Value);
                    var elapsedSeconds = Convert.ToInt32(elapsed.TotalSeconds);
                    var continueChatTimeout = ChatController.ContinueChatTimeout();
                    if (elapsedSeconds > continueChatTimeout)
                    {
                        LoggingService.GetInstance().LogNote(String.Format("Continue Chat timed out for connection: {0}", webChat.ConnectionId));
                        Disconnect(webChat, ChatServices.DisconnectReason.ContinueChatTimeout);
                        return;
                    }
                }
            }
            if ((webChat.State == ChatState.Connected || webChat.State == ChatState.Paused) && ChatController.EnableInactivityTimeout() && webChat.DateAnswered.HasValue)
            {
                if (ChatController.InactivityTimeout() > 0)
                {
                    var inactiveTimeout = ChatController.InactivityTimeout();
                    var lastAgent = webChat.Messages.OrderBy(m => m.Order).LastOrDefault(m => m.Direction == "in" && !m.Name.Equals("System", StringComparison.OrdinalIgnoreCase));
                    var lastUser = webChat.Messages.OrderBy(m => m.Order).LastOrDefault(m => m.Direction == "out" && !m.Name.Equals("System", StringComparison.OrdinalIgnoreCase));
                    var now = DateTime.Now;
                    var last = now;
                    if (lastAgent != null && lastUser != null)
                    {
                        if (ChatController.EnableInactivityResetOnAgentMessage())
                        {
                            last = lastAgent.DateSent > lastUser.DateSent ? lastAgent.DateSent : lastUser.DateSent;
                        }
                        else
                        {
                            last = lastUser.DateSent;
                        }
                    }
                    else if(lastAgent != null)
                    {
                        last = lastAgent.DateSent;
                    }
                    if (last != now)
                    {
                        var elapsed = now.Subtract(last);
                        var elapsedSeconds = Convert.ToInt32(elapsed.TotalSeconds);
                        if (elapsedSeconds > inactiveTimeout)
                        {
                            LoggingService.GetInstance().LogNote(String.Format("Inactivity Time Out Occurred: {0}", webChat.ConnectionId));
                            Disconnect(webChat, ChatServices.DisconnectReason.InactivityTimeout);
                            return;
                        }
                    }
                }
            }
            ChatServices.PollEvents(webChat.ParticipantId, webChat.ConnectionId, webChat.SessionId);
        }

        void PollVisitorMessage(VisitorMessageChat visitorMessage)
        {
            if (ChatServices.VisitorMessageChats.Any(v => v.VisitorMessageId == visitorMessage.VisitorMessageId))
            {
                ChatServices.PollVisitorMessageEvents(visitorMessage);
            }
        }

        void CheckPartyInfo(long chatId, string participantId, string agentParticipantId)
        {
            ChatServices.GetPartyInfo(chatId, participantId, agentParticipantId);
        }

        public void Disconnect(WebChat webChat, ChatServices.DisconnectReason reason)
        {
            try
            {
                var chatId = webChat.ChatId;
                var connectionId = webChat.ConnectionId;
                var participantId = webChat.ParticipantId;
                var previousConnectionIds = webChat.PreviousConnectionIds;
                if (ChatController.EnableKeepOpenOnDisconnectAndStartNew() && webChat.DateAnswered.HasValue)
                {
                    if (webChat.Messages.Any() && !ChatServices.DisconnectedWebChats.Any(
                            d =>
                                d.ConnectionId == webChat.ConnectionId &&
                                (d.PreviousConnectionIds != null &&
                                 d.PreviousConnectionIds.Contains(webChat.ConnectionId))))
                    {
                        ChatServices.DisconnectedWebChats.Add(webChat);
                    }
                }
                ChatServices.WebChats.Remove(webChat);
                ChatServices.Disconnect(connectionId, reason);
                if (previousConnectionIds != null)
                {
                    foreach (var previousConnectionId in previousConnectionIds)
                    {
                        ChatServices.Disconnect(previousConnectionId, reason, "", true, true);
                    }
                }
                ChatServices.EndChat(participantId);
                var repository = new Repository();
                var chat = repository.Chats.Find(chatId);
                if (chat != null)
                {
                    chat.DateEnded = DateTime.Now;
                    repository.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
        }


        public void Stop(bool immediate)
        {
            TaskTimer.Dispose();
            _running = false;
            HostingEnvironment.UnregisterObject(this);
        }
    }
}