using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;
using WebchatBuilder.Helpers;
using WebchatBuilder.Services;
using WebchatBuilder.ViewModels;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;
using FieldType = WebChatBuilderModels.Models.FieldType;

namespace WebchatBuilder.Controllers
{
    [AuthorizeIpAddress]
    [Authorize(Roles = "SettingsAdmin")]
    public class SettingsController : Controller
    {
        private readonly Repository _repository = new Repository();

        [HttpGet]
        public ActionResult Settings()
        {
            var model = new SettingsViewModel();
            try
            {
                var company = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "companyname");
                var logo = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "logopath");

                if (company == null)
                {
                    company = new Setting
                    {
                        Key = "CompanyName",
                        Value = "CompanyName"
                    };
                    _repository.Settings.Add(company);
                    _repository.SaveChanges();
                }
                if (logo == null)
                {
                    logo = new Setting
                    {
                        Key = "LogoPath",
                        Value = ""
                    };
                    _repository.Settings.Add(logo);
                    _repository.SaveChanges();
                }
                var continueChat = ChatController.ContinueChat();
                var defaultUser = ChatController.GetUserName("");
                var defaultAgentName = ChatController.GetAgentName();
                var overrideAgentName = ChatController.OverrideAgent();
                var enableSendAndQueue = ChatController.EnableSendAndQueue();
                var saveIp = ChatController.SaveIpAddress();
                var chatTimeout = ChatController.ContinueChatTimeout();
                var allowIframes = ChatController.Iframes();
                var dropInactive = ChatController.EnableInactivityTimeout();
                var resetActivityOnAgent = ChatController.EnableInactivityResetOnAgentMessage();
                var inactiveTimeout = ChatController.InactivityTimeout();
                var blockSystemMessages = ChatController.BlockSystemMessages();
                var customSystemMessages = ChatController.EnableCustomSystemMessages();
                var keepOpenOnDq = ChatController.EnableKeepOpenOnDisconnectAndStartNew();
                var errorText = ChatController.GetCustomErrorMessage();
                var sendToAgentOnRestart = ChatController.EnablePassHistoryToNewAgentOnRestart();
                var connectionLostText = ChatController.GetConnectionLostText();
                var loggingEnabled = ChatController.EnableLogging();
                var reloadUserHistory = ChatController.EnableReloadUserHistoryOnNewChat();
                var reloadUnanswered = ChatController.EnableReloadUnansweredChatHistory();
                var audioAlertsEnabled = ChatController.EnableAudioAlerts;
                var browserAlertsEnabled = ChatController.EnableBrowserAlerts;
                var showOptionsButton = ChatController.ShowOptionsButton;
                //var enableHardDisconnect = ChatController.EnableHardDisconnect;
                var showUserAgentInUserData = ChatController.ShowUserAgentInUserData;
                //var transferTimeout = ChatController.TransferTimeout;
                var emailTranscriptSubject = ChatController.EmailTranscriptSubject;
                var closeButtonTitle = ChatController.CloseButtonTitle;
                var disconnectButtonTitle = ChatController.DisconnectButtonTitle;
                var keepQueuedChatsAlive = ChatController.KeepQueuedChatsAlive;
                var showCustomInfoOnReload = ChatController.ShowCustomInfoOnReload;
                var showCustomInfoOnLoad = ChatController.ShowCustomInfoOnLoad;
                var enableGoogleAnalytics = ChatController.EnableGoogleAnalytics;
                var googleAnalyticsTrackingId = ChatController.GoogleAnalyticsTrackingId;

                model = new SettingsViewModel
                {
                    GeneralSettingsViewModel = new GeneralSettingsViewModel
                    {
                        CompanyName = company.Value,
                        Logo = logo.Value,
                        Logos = GetLogos(),
                        ContinueChat = continueChat,
                        DefaultUserName = defaultUser,
                        SaveIpAddress = saveIp,
                        ChatTimeout = chatTimeout,
                        Iframes = allowIframes,
                        DefaultAgentName = defaultAgentName,
                        OverrideAgentName = overrideAgentName,
                        EnableSendAndQueue = enableSendAndQueue,
                        DropInactiveChats = dropInactive,
                        ResetActivityTimeoutOnAgentMessage = resetActivityOnAgent,
                        InactiveChatTimeout = inactiveTimeout,
                        BlockCicSystemMessages = blockSystemMessages,
                        UseCustomSystemMessages = customSystemMessages,
                        KeepOpenOnDisconnectAndStartNew = keepOpenOnDq,
                        CustomErrorText = errorText,
                        PassHistoryToNewAgentOnRestart = sendToAgentOnRestart,
                        LoggingEnabled = loggingEnabled,
                        ConnectionLostText = connectionLostText,
                        ReloadUserHistoryOnNewChat = reloadUserHistory,
                        ReloadUnansweredChatHistory = reloadUnanswered,
                        AudioAlertsEnabled = audioAlertsEnabled,
                        BrowserAlertsEnabled = browserAlertsEnabled,
                        ShowOptionsButton = showOptionsButton,
                        //EnableHardDisconnect = enableHardDisconnect,
                        ShowUserAgentInUserData = showUserAgentInUserData,
                        //TransferTimeout = transferTimeout
                        EmailTranscriptSubject = emailTranscriptSubject,
                        CloseButtonTitle = closeButtonTitle,
                        DisconnectButtonTitle = disconnectButtonTitle,
                        KeepQueuedChatsAlive = keepQueuedChatsAlive,
                        ShowCustomInfoOnReload = showCustomInfoOnReload,
                        ShowCustomInfoOnLoad = showCustomInfoOnLoad,
                        GoogleAnalyticsTrackingId = googleAnalyticsTrackingId,
                        EnableGoogleAnalytics = enableGoogleAnalytics
                    },
                    SkillsSettingsViewModel = new SkillsSettingsViewModel
                    {
                        AssignableSkills = _repository.Skills.Where(s => s.IsAssignable && !s.MarkedForDeletion).ToList().Select(x => new SkillSettingsViewModel
                        {
                            SkillId = x.SkillId,
                            DisplayName = x.DisplayName,
                            IsUsed = x.IsUsed
                        }).ToList(),
                        UnassignableSkills = _repository.Skills.Where(s => !s.IsAssignable && !s.MarkedForDeletion).ToList().Select(x => new SkillSettingsViewModel
                        {
                            SkillId = x.SkillId,
                            DisplayName = x.DisplayName,
                            IsUsed = x.IsUsed
                        }).ToList()
                    },
                    WorkgroupsSettingsViewModel = new WorkgroupsSettingsViewModel
                    {
                        AssignableWorkgroups = _repository.Workgroups.Where(s => s.IsAssignable && !s.MarkedForDeletion).ToList().Select(x => new WorkgroupSettingViewModel
                        {
                            WorkgroupId = x.WorkgroupId,
                            DisplayName = x.DisplayName,
                            IsUsed = x.IsUsed
                        }).ToList(),
                        UnassignableWorkgroups = _repository.Workgroups.Where(s => !s.IsAssignable && !s.MarkedForDeletion).ToList().Select(x => new WorkgroupSettingViewModel
                        {
                            WorkgroupId = x.WorkgroupId,
                            DisplayName = x.DisplayName,
                            IsUsed = x.IsUsed
                        }).ToList()
                    },
                    TemplateSettingsViewModel = new TemplateSettingsViewModel
                    {
                        Templates = _repository.Templates.ToList()
                    },
                    WidgetSettingsViewModel = new WidgetSettingsViewModel
                    {
                        Widgets = _repository.Widgets.ToList(),
                        Icons = GetLaunchIcons(),
                        Forms = _repository.Forms.ToList(),
                        UseCallbackForVisitorMessages = ChatController.UseCallbackForVMs(),
                        UseProfileWorkgroupForVisitorMessages = ChatController.UseProfileWorkgroupForVMs(),
                        DefaultWorkgroupForVisitorMessages = ChatController.GetDefaultWorkgroupForVisitorMessages(),
                        ConfirmNameInVisitorMessages = ChatController.ConfirmNameInVisitorMessagesVoicemail()
                    },
                    ScheduleSettingsViewModel = new ScheduleSettingsViewModel
                    {
                        Schedules = _repository.Schedules.ToList().Select(i => new ScheduleListViewModel
                        {
                            IsAssignable = i.IsAssignable,
                            MarkedForDeletion = i.MarkedForDeletion,
                            ClosedOnly = i.ClosedOnly,
                            Name = i.DisplayName,
                            ScheduleId = i.Id,
                            IsUsed = i.IsUsed
                        }).ToList()
                    }
                };
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return View(model);
        }

        private ICollection<string> GetLogos()
        {
            try
            {
                return Directory.EnumerateFiles(Server.MapPath("~/Images/Logos")).Select(fn => "/Images/Logos/" + Path.GetFileName(fn)).ToList();
            }
            catch (Exception)
            {
                return new Collection<string>();
            }
        }

        [HttpPost]
        public ActionResult AddLogo()
        {
            var message = "";
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/Logos"), fileName);
                        file.SaveAs(path);
                        return Json(new {success = true, imgUrl = "/Images/Logos/" + fileName});
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult CompanyInfo(GeneralSettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var company = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "companyname");
                    var logo = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "logopath");
                    if (company != null && logo != null)
                    {
                        company.Value = model.CompanyName;
                        logo.Value = model.Logo;
                        _repository.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

            }
            var continueChat = ChatController.ContinueChat();
            var defaultUser = ChatController.GetUserName("");
            var defaultAgentName = ChatController.GetAgentName();
            var overrideAgentName = ChatController.OverrideAgent();
            var enableSendAndQueue = ChatController.EnableSendAndQueue();
            var saveIp = ChatController.SaveIpAddress();
            var chatTimeout = ChatController.ContinueChatTimeout();
            var dropInactive = ChatController.EnableInactivityTimeout();
            var resetActivityOnAgent = ChatController.EnableInactivityResetOnAgentMessage();
            var inactiveTimeout = ChatController.InactivityTimeout();
            var blockSystemMessages = ChatController.BlockSystemMessages();
            var customSystemMessages = ChatController.EnableCustomSystemMessages();
            var keepOpenOnDisconnectAndStartNew = ChatController.EnableKeepOpenOnDisconnectAndStartNew();
            var errorText = ChatController.GetCustomErrorMessage();
            var sendToAgentOnRestart = ChatController.EnablePassHistoryToNewAgentOnRestart();
            var connectionLostText = ChatController.GetConnectionLostText();
            var loggingEnabled = ChatController.EnableLogging();
            var reloadUserHistory = ChatController.EnableReloadUserHistoryOnNewChat();
            var reloadUnanswered = ChatController.EnableReloadUnansweredChatHistory();
            var audioAlertsEnabled = ChatController.EnableAudioAlerts;
            var browserAlertsEnabled = ChatController.EnableBrowserAlerts;
            var showOptionsButton = ChatController.ShowOptionsButton;
            //var enableHardDisconnect = ChatController.EnableHardDisconnect;
            var showUserAgentInUserData = ChatController.ShowUserAgentInUserData;
            var emailTranscriptSubject = ChatController.EmailTranscriptSubject;
            var closeButtonTitle = ChatController.CloseButtonTitle;
            var disconnectButtonTitle = ChatController.DisconnectButtonTitle;
            var keepQueuedChatsAlive = ChatController.KeepQueuedChatsAlive;
            var showCustomInfoOnReload = ChatController.ShowCustomInfoOnReload;
            var showCustomInfoOnLoad = ChatController.ShowCustomInfoOnLoad;
            var enableGoogleAnalytics = ChatController.EnableGoogleAnalytics;
            var googleAnalyticsTrackingId = ChatController.GoogleAnalyticsTrackingId;
            //var transferTimeout = ChatController.TransferTimeout;

            model.SaveIpAddress = saveIp;
            model.DefaultUserName = defaultUser;
            model.ContinueChat = continueChat;
            model.ChatTimeout = chatTimeout;
            model.OverrideAgentName = overrideAgentName;
            model.DefaultAgentName = defaultAgentName;
            model.EnableSendAndQueue = enableSendAndQueue;
            model.DropInactiveChats = dropInactive;
            model.ResetActivityTimeoutOnAgentMessage = resetActivityOnAgent;
            model.InactiveChatTimeout = inactiveTimeout;
            model.BlockCicSystemMessages = blockSystemMessages;
            model.UseCustomSystemMessages = customSystemMessages;
            model.KeepOpenOnDisconnectAndStartNew = keepOpenOnDisconnectAndStartNew;
            model.CustomErrorText = errorText;
            model.PassHistoryToNewAgentOnRestart = sendToAgentOnRestart;
            model.ConnectionLostText = connectionLostText;
            model.LoggingEnabled = loggingEnabled;
            model.ReloadUserHistoryOnNewChat = reloadUserHistory;
            model.ReloadUnansweredChatHistory = reloadUnanswered;
            model.AudioAlertsEnabled = audioAlertsEnabled;
            model.BrowserAlertsEnabled = browserAlertsEnabled;
            model.ShowOptionsButton = showOptionsButton;
            model.ShowUserAgentInUserData = showUserAgentInUserData;
            model.EmailTranscriptSubject = emailTranscriptSubject;
            model.CloseButtonTitle = closeButtonTitle;
            model.DisconnectButtonTitle = disconnectButtonTitle;
            model.KeepQueuedChatsAlive = keepQueuedChatsAlive;
            model.ShowCustomInfoOnReload = showCustomInfoOnReload;
            model.ShowCustomInfoOnLoad = showCustomInfoOnLoad;
            model.EnableGoogleAnalytics = enableGoogleAnalytics;
            model.GoogleAnalyticsTrackingId = googleAnalyticsTrackingId;
            //model.EnableHardDisconnect = enableHardDisconnect;
            //model.TransferTimeout = transferTimeout;
            model.Logos = GetLogos();
            if (model.Logo == null)
            {
                model.Logo = "";
            }
            return PartialView("_General", model);
        }

        [HttpPost]
        public ActionResult GlobalChatSettings(GeneralSettingsViewModel model)
        {
            try
            {
                var cont = _repository.Settings.FirstOrDefault(s => s.Key == "AllowContinueChat");
                var ipAddr = _repository.Settings.FirstOrDefault(s => s.Key == "GatherIPAddress");
                var user = _repository.Settings.FirstOrDefault(s => s.Key == "DefaultUserName");
                var agent = _repository.Settings.FirstOrDefault(s => s.Key == "DefaultAgentName");
                var overrideAgent = _repository.Settings.FirstOrDefault(s => s.Key == "OverrideAgentName");
                var sendAndQueue = _repository.Settings.FirstOrDefault(s => s.Key == "EnableSendAndQueueChatsBeforeAgent");
                var timeout = _repository.Settings.FirstOrDefault(s => s.Key == "ContinueChatTimeout");
                var iframes = _repository.Settings.FirstOrDefault(s => s.Key == "AllowIframes");
                var dropInactive = _repository.Settings.FirstOrDefault(s => s.Key == "DropInactiveChats");
                var resetActivityOnAgent = _repository.Settings.FirstOrDefault(s => s.Key == "ResetActivityTimeoutOnAgentMessage");
                var inactiveTimeout = _repository.Settings.FirstOrDefault(s => s.Key == "InactiveChatTimeout");
                var blockSystemMessages = _repository.Settings.FirstOrDefault(s => s.Key == "BlockCicSystemMessages");
                var customSystemMessages = _repository.Settings.FirstOrDefault(s => s.Key == "UseCustomSystemMessages");
                var keepOpenOnDisconnect = _repository.Settings.FirstOrDefault(s => s.Key == "KeepOpenOnDisconnectAndStartNew");
                var customErrorText = _repository.Settings.FirstOrDefault(s => s.Key == "CustomErrorText");
                var sendToAgentOnRestart = _repository.Settings.FirstOrDefault(s => s.Key == "PassHistoryToNewAgentOnRestart");
                var connectionLost = _repository.Settings.FirstOrDefault(s => s.Key == "ConnectionLostText");
                var logging = _repository.Settings.FirstOrDefault(s => s.Key == "LoggingEnabled");
                var reloadUserHistory = _repository.Settings.FirstOrDefault(s => s.Key == "ReloadUserHistoryOnNewChat");
                var reloadUnanswered = _repository.Settings.FirstOrDefault(s => s.Key == "ReloadUnansweredChatHistory");

                if (cont != null && ipAddr != null && user != null && timeout != null && iframes != null && agent != null && overrideAgent != null && sendAndQueue != null && dropInactive != null && resetActivityOnAgent != null && inactiveTimeout != null && blockSystemMessages != null && customSystemMessages != null && keepOpenOnDisconnect != null && customErrorText != null && sendToAgentOnRestart != null && logging != null && connectionLost != null && reloadUserHistory != null && reloadUnanswered != null)
                {
                    ChatController.AllowContinueChat = model.ContinueChat;
                    ChatController.DefaultUserName = model.DefaultUserName;
                    ChatController.GatherIpAddress = model.SaveIpAddress;
                    ChatController.PausedChatTimeout = model.ChatTimeout;
                    ChatController.AllowIframes = model.Iframes;
                    ChatController.DefaultAgentName = model.DefaultAgentName;
                    ChatController.OverrideAgentName = model.OverrideAgentName;
                    ChatController.EnableSendAndQueueChatsBeforeAgent = model.EnableSendAndQueue;
                    ChatController.DropInactiveChats = model.DropInactiveChats;
                    ChatController.ResetActivityTimeoutOnAgentMessage = model.ResetActivityTimeoutOnAgentMessage;
                    ChatController.InactiveChatTimeout = model.InactiveChatTimeout;
                    ChatController.BlockCicSystemMessages = model.BlockCicSystemMessages;
                    ChatController.UseCustomSystemMessages = model.UseCustomSystemMessages;
                    ChatController.KeepOpenOnDisconnectAndStartNew = model.KeepOpenOnDisconnectAndStartNew;
                    ChatController.CustomErrorText = model.CustomErrorText;
                    ChatController.PassHistoryToNewAgentOnRestart = model.PassHistoryToNewAgentOnRestart;
                    ChatController.ConnectionLostText = model.ConnectionLostText;
                    ChatController.LoggingEnabled = model.LoggingEnabled;
                    ChatController.ReloadUserHistoryOnNewChat = model.ReloadUserHistoryOnNewChat;
                    ChatController.ReloadUnansweredChatHistory = model.ReloadUnansweredChatHistory;
                    //New
                    ChatController.EnableAudioAlerts = model.AudioAlertsEnabled;
                    ChatController.EnableBrowserAlerts = model.BrowserAlertsEnabled;
                    ChatController.ShowOptionsButton = model.ShowOptionsButton;
                    //ChatController.EnableHardDisconnect = model.EnableHardDisconnect;
                    ChatController.ShowUserAgentInUserData = model.ShowUserAgentInUserData;
                    ChatController.EmailTranscriptSubject = model.EmailTranscriptSubject;
                    ChatController.CloseButtonTitle = model.CloseButtonTitle;
                    ChatController.DisconnectButtonTitle = model.DisconnectButtonTitle;
                    //ChatController.TransferTimeout = model.TransferTimeout;
                    ChatController.KeepQueuedChatsAlive = model.KeepQueuedChatsAlive;
                    ChatController.ShowCustomInfoOnReload = model.ShowCustomInfoOnReload;
                    ChatController.ShowCustomInfoOnLoad = model.ShowCustomInfoOnLoad;
                    ChatController.EnableGoogleAnalytics = model.EnableGoogleAnalytics;
                    ChatController.GoogleAnalyticsTrackingId = model.GoogleAnalyticsTrackingId;

                    cont.Value = model.ContinueChat.ToString();
                    ipAddr.Value = model.SaveIpAddress.ToString();
                    user.Value = model.DefaultUserName;
                    timeout.Value = model.ChatTimeout.ToString();
                    agent.Value = model.DefaultAgentName;
                    overrideAgent.Value = model.OverrideAgentName.ToString();
                    sendAndQueue.Value = model.EnableSendAndQueue.ToString();
                    dropInactive.Value = model.DropInactiveChats.ToString();
                    resetActivityOnAgent.Value = model.ResetActivityTimeoutOnAgentMessage.ToString();
                    inactiveTimeout.Value = model.InactiveChatTimeout.ToString();
                    blockSystemMessages.Value = model.BlockCicSystemMessages.ToString();
                    customSystemMessages.Value = model.UseCustomSystemMessages.ToString();
                    keepOpenOnDisconnect.Value = model.KeepOpenOnDisconnectAndStartNew.ToString();
                    customErrorText.Value = model.CustomErrorText;
                    sendToAgentOnRestart.Value = model.PassHistoryToNewAgentOnRestart.ToString();
                    logging.Value = model.LoggingEnabled.ToString();
                    connectionLost.Value = model.ConnectionLostText;
                    reloadUserHistory.Value = model.ReloadUserHistoryOnNewChat.ToString();
                    reloadUnanswered.Value = model.ReloadUnansweredChatHistory.ToString();
                    _repository.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }
            var company = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "companyname");
            var logo = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "logopath");
            if (company != null)
            {
                model.CompanyName = company.Value;
            }
            if (logo != null)
            {
                model.Logo = logo.Value;
            }
            var continueChat = ChatController.ContinueChat();
            var defaultUser = ChatController.GetUserName("");
            var saveIp = ChatController.SaveIpAddress();
            var chatTimeout = ChatController.ContinueChatTimeout();
            var allowIframes = ChatController.Iframes();
            var defaultAgentName = ChatController.GetAgentName();
            var overrideAgentName = ChatController.OverrideAgent();
            var enableSendAndQueue = ChatController.EnableSendAndQueue();
            var dropInactivechats = ChatController.EnableInactivityTimeout();
            var resetTimeoutActivityOnAgent = ChatController.EnableInactivityResetOnAgentMessage();
            var inactivityTimeout = ChatController.InactivityTimeout();
            var blockCicSystemMessages = ChatController.BlockSystemMessages();
            var customSysMessages = ChatController.EnableCustomSystemMessages();
            var keepOpen = ChatController.EnableKeepOpenOnDisconnectAndStartNew();
            var errorText = ChatController.GetCustomErrorMessage();
            var passHistory = ChatController.EnablePassHistoryToNewAgentOnRestart();
            var connectionLostText = ChatController.GetConnectionLostText();
            var loggingEnabled = ChatController.EnableLogging();
            var reloadHistory = ChatController.EnableReloadUserHistoryOnNewChat();
            var keepUnanswered = ChatController.EnableReloadUnansweredChatHistory();
            model.SaveIpAddress = saveIp;
            model.DefaultUserName = defaultUser;
            model.ContinueChat = continueChat;
            model.ChatTimeout = chatTimeout;
            model.Iframes = allowIframes;
            model.OverrideAgentName = overrideAgentName;
            model.DefaultAgentName = defaultAgentName;
            model.EnableSendAndQueue = enableSendAndQueue;
            model.Logos = GetLogos();
            model.DropInactiveChats = dropInactivechats;
            model.ResetActivityTimeoutOnAgentMessage = resetTimeoutActivityOnAgent;
            model.InactiveChatTimeout = inactivityTimeout;
            model.BlockCicSystemMessages = blockCicSystemMessages;
            model.UseCustomSystemMessages = customSysMessages;
            model.KeepOpenOnDisconnectAndStartNew = keepOpen;
            model.CustomErrorText = errorText;
            model.PassHistoryToNewAgentOnRestart = passHistory;
            model.ReloadUserHistoryOnNewChat = reloadHistory;
            model.LoggingEnabled = loggingEnabled;
            model.ConnectionLostText = connectionLostText;
            model.ReloadUnansweredChatHistory = keepUnanswered;
            model.AudioAlertsEnabled = ChatController.EnableAudioAlerts;
            model.BrowserAlertsEnabled = ChatController.EnableBrowserAlerts;
            model.ShowOptionsButton = ChatController.ShowOptionsButton;
            model.ShowUserAgentInUserData = ChatController.ShowUserAgentInUserData;
            model.EmailTranscriptSubject = ChatController.EmailTranscriptSubject;
            model.CloseButtonTitle = ChatController.CloseButtonTitle;
            model.DisconnectButtonTitle = ChatController.DisconnectButtonTitle;
            //model.EnableHardDisconnect = ChatController.EnableHardDisconnect;
            //model.TransferTimeout = ChatController.TransferTimeout;
            model.KeepQueuedChatsAlive = ChatController.KeepQueuedChatsAlive;
            model.ShowCustomInfoOnReload = ChatController.ShowCustomInfoOnReload;
            model.ShowCustomInfoOnLoad = ChatController.ShowCustomInfoOnLoad;
            model.EnableGoogleAnalytics = ChatController.EnableGoogleAnalytics;
            model.GoogleAnalyticsTrackingId = ChatController.GoogleAnalyticsTrackingId;

            return PartialView("_General", model);
        }

        public JsonResult UpdateCustomSystemMessages(CustomMessagesViewModel model)
        {
            try
            {
                var connected = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.Connected);
                var agentJoined = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.AgentJoined);
                var agentLeft = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.AgentDisconnect);
                var inactive = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.InactiveDisconnect);
                var restarted = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.RestartedChat);
                var paused = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.PausedChat);
                var resumed = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.ResumedChat);
                var visitorDisconnect = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.VisitorDisconnect);
                if (connected != null)
                {
                    connected.Message = model.ConnectedMessage;
                    connected.IsEnabled = model.ConnectedMessageEnabled;
                }
                if (agentJoined != null)
                {
                    agentJoined.Message = model.AgentJoinedMessage;
                    agentJoined.IsEnabled = model.AgentJoinedMessageEnabled;
                }
                if (agentLeft != null)
                {
                    agentLeft.Message = model.AgentDisconnectMessage;
                    agentLeft.IsEnabled = model.AgentDisconnectMessageEnabled;
                }
                if (inactive != null)
                {
                    inactive.Message = model.InactiveDisconnectMessage;
                    inactive.IsEnabled = model.InactiveDisconnectMessageEnabled;
                }
                if (restarted != null)
                {
                    restarted.Message = model.RestartedChatMessage;
                    restarted.IsEnabled = model.RestartedChatMessageEnabled;
                }
                if (paused != null)
                {
                    paused.Message = model.PausedChatMessage;
                    paused.IsEnabled = model.PausedChatMessageEnabled;
                }
                if (resumed != null)
                {
                    resumed.Message = model.ResumedChatMessage;
                    resumed.IsEnabled = model.ResumedChatMessageEnabled;
                }
                if (visitorDisconnect != null)
                {
                    visitorDisconnect.Message = model.VisitorDisconnectMessage;
                    visitorDisconnect.IsEnabled = model.VisitorDisconnectMessageEnabled;
                }
                _repository.SaveChanges();
                return Json(new {success = true});
            }
            catch (Exception)
            {
            }
            return Json(new {success = false});
        }

        public ActionResult GetCustomSystemMessages()
        {
            try
            {
                var model = DefaultCustomMessagesViewModel();
                var connected = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.Connected);
                var agentJoined = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.AgentJoined);
                var agentLeft = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.AgentDisconnect);
                var inactive = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.InactiveDisconnect);
                var restarted = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.RestartedChat);
                var paused = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.PausedChat);
                var resumed = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.ResumedChat);
                var visitorDisconnect = _repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.VisitorDisconnect);

                if (connected == null)
                {
                    connected = new CustomMessage
                    {
                        Name = "Connected",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.Connected
                    };
                    _repository.CustomMessages.Add(connected);
                    _repository.SaveChanges();
                }
                if (agentJoined == null)
                {
                    agentJoined = new CustomMessage
                    {
                        Name = "Agent Joined",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.AgentJoined
                    };
                    _repository.CustomMessages.Add(agentJoined);
                    _repository.SaveChanges();
                }
                if (agentLeft == null)
                {
                    agentLeft = new CustomMessage
                    {
                        Name = "Agent Disconnected",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.AgentDisconnect
                    };
                    _repository.CustomMessages.Add(agentLeft);
                    _repository.SaveChanges();
                }
                if (inactive == null)
                {
                    inactive = new CustomMessage
                    {
                        Name = "Inactive",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.InactiveDisconnect
                    };
                    _repository.CustomMessages.Add(inactive);
                    _repository.SaveChanges();
                }
                if (restarted == null)
                {
                    restarted = new CustomMessage
                    {
                        Name = "Restarted",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.RestartedChat
                    };
                    _repository.CustomMessages.Add(restarted);
                    _repository.SaveChanges();
                }
                if (paused == null)
                {
                    paused = new CustomMessage
                    {
                        Name = "Paused",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.PausedChat
                    };
                    _repository.CustomMessages.Add(paused);
                    _repository.SaveChanges();
                }
                if (resumed == null)
                {
                    resumed = new CustomMessage
                    {
                        Name = "Resumed",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.ResumedChat
                    };
                    _repository.CustomMessages.Add(resumed);
                    _repository.SaveChanges();
                }
                if (visitorDisconnect == null)
                {
                    visitorDisconnect = new CustomMessage
                    {
                        Name = "Visitor Disconnected",
                        IsEnabled = false,
                        Message = "",
                        Type = CustomMessageType.VisitorDisconnect
                    };
                    _repository.CustomMessages.Add(visitorDisconnect);
                    _repository.SaveChanges();
                }

                model.AgentDisconnectMessage = agentLeft.Message;
                model.AgentDisconnectMessageEnabled = agentLeft.IsEnabled;
                model.AgentJoinedMessage = agentJoined.Message;
                model.AgentJoinedMessageEnabled = agentJoined.IsEnabled;
                model.ConnectedMessage = connected.Message;
                model.ConnectedMessageEnabled = connected.IsEnabled;
                model.InactiveDisconnectMessage = inactive.Message;
                model.InactiveDisconnectMessageEnabled = inactive.IsEnabled;
                model.PausedChatMessage = paused.Message;
                model.PausedChatMessageEnabled = paused.IsEnabled;
                model.RestartedChatMessage = restarted.Message;
                model.RestartedChatMessageEnabled = restarted.IsEnabled;
                model.ResumedChatMessage = resumed.Message;
                model.ResumedChatMessageEnabled = resumed.IsEnabled;
                model.VisitorDisconnectMessage = visitorDisconnect.Message;
                model.VisitorDisconnectMessageEnabled = visitorDisconnect.IsEnabled;
                return PartialView("_CustomMessages", model);
            }
            catch (Exception)
            {
            }
            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        private CustomMessagesViewModel DefaultCustomMessagesViewModel()
        {
            return new CustomMessagesViewModel
            {
                ConnectedMessage = "",
                ConnectedMessageEnabled = false,
                AgentJoinedMessage = "",
                AgentJoinedMessageEnabled = false,
                AgentDisconnectMessage = "",
                AgentDisconnectMessageEnabled = false,
                InactiveDisconnectMessage = "",
                InactiveDisconnectMessageEnabled = false,
                PausedChatMessage = "",
                PausedChatMessageEnabled = false,
                RestartedChatMessage = "",
                RestartedChatMessageEnabled = false,
                ResumedChatMessage = "",
                ResumedChatMessageEnabled = false,
                VisitorDisconnectMessage = "",
                VisitorDisconnectMessageEnabled = false
            };
        }

        public ActionResult ToggleSkillAssignable(int id)
        {
            try
            {
                var skill = _repository.Skills.FirstOrDefault(w => w.SkillId == id);
                if (skill != null)
                {
                    skill.IsAssignable = !skill.IsAssignable;
                    _repository.SaveChanges();
                    var model = new SkillsSettingsViewModel
                    {
                        AssignableSkills = _repository.Skills.Where(s => s.IsAssignable && !s.MarkedForDeletion).ToList().Select(x => new SkillSettingsViewModel
                        {
                            SkillId = x.SkillId,
                            DisplayName = x.DisplayName,
                            IsUsed = x.IsUsed
                        }).ToList(),
                        UnassignableSkills = _repository.Skills.Where(s => !s.IsAssignable && !s.MarkedForDeletion).ToList().Select(x => new SkillSettingsViewModel
                        {
                            SkillId = x.SkillId,
                            DisplayName = x.DisplayName,
                            IsUsed = x.IsUsed
                        }).ToList()
                    };
                    return PartialView("_Skills", model);
                }
            }
            catch (Exception)
            {
            }

            return Json(new {success = false});
        }

        public ActionResult ToggleWorkgroupAssignable(int id)
        {
            try
            {
                var workgroup = _repository.Workgroups.FirstOrDefault(w => w.WorkgroupId == id);
                if (workgroup != null)
                {
                    workgroup.IsAssignable = !workgroup.IsAssignable;
                    _repository.SaveChanges();
                    var model = new WorkgroupsSettingsViewModel
                    {
                        AssignableWorkgroups =
                            _repository.Workgroups.Where(s => s.IsAssignable && !s.MarkedForDeletion)
                                .ToList()
                                .Select(x => new WorkgroupSettingViewModel
                                {
                                    WorkgroupId = x.WorkgroupId,
                                    DisplayName = x.DisplayName,
                                    IsUsed = x.IsUsed
                                }).ToList(),
                        UnassignableWorkgroups =
                            _repository.Workgroups.Where(s => !s.IsAssignable && !s.MarkedForDeletion)
                                .ToList()
                                .Select(x => new WorkgroupSettingViewModel
                                {
                                    WorkgroupId = x.WorkgroupId,
                                    DisplayName = x.DisplayName,
                                    IsUsed = x.IsUsed
                                }).ToList()
                    };
                    return PartialView("_Workgroups", model);
                }
            }
            catch (Exception)
            {
            }

            return Json(new {success = false});
        }

        [HttpGet]
        public ActionResult ReloadSchedules()
        {
            var model = new ScheduleSettingsViewModel
            {
                Schedules = _repository.Schedules.ToList().Select(i => new ScheduleListViewModel
                {
                    IsAssignable = i.IsAssignable,
                    MarkedForDeletion = i.MarkedForDeletion,
                    ClosedOnly = i.ClosedOnly,
                    Name = i.DisplayName,
                    ScheduleId = i.Id,
                    IsUsed = i.IsUsed
                }).ToList()
            };
            return PartialView("_Schedules", model);
        }

        [HttpPost]
        public ActionResult ToggleScheduleAssignable(int id)
        {
            try
            {
                var schedule = _repository.Schedules.Find(id);
                if (schedule != null)
                {
                    schedule.IsAssignable = !schedule.IsAssignable;
                    _repository.SaveChanges();
                    var model = new ScheduleSettingsViewModel
                    {
                        Schedules = _repository.Schedules.ToList().Select(i => new ScheduleListViewModel
                        {
                            IsAssignable = i.IsAssignable,
                            MarkedForDeletion = i.MarkedForDeletion,
                            ClosedOnly = i.ClosedOnly,
                            Name = i.DisplayName,
                            ScheduleId = i.Id,
                            IsUsed = i.IsUsed
                        }).ToList()
                    };
                    return PartialView("_Schedules", model);
                }
            }
            catch (Exception)
            {
            }

            return Json(new {success = false});
        }

        [HttpGet]
        public ActionResult EditSchedule(int id)
        {
            try
            {
                var schedule = _repository.Schedules.Find(id);
                if (schedule != null)
                {
                    var model = new EditScheduleViewModel
                    {
                        ScheduleId = schedule.Id,
                        ClosedOnly = schedule.ClosedOnly,
                        Name = schedule.DisplayName,
                        OverrideMessage = schedule.OverrideMessage,
                        Description = schedule.Description
                    };
                    return PartialView("_EditSchedule", model);
                }
            }
            catch (Exception)
            {
            }

            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditSchedule(EditScheduleViewModel model)
        {
            try
            {
                var schedule = _repository.Schedules.Find(model.ScheduleId);
                if (schedule != null)
                {
                    schedule.ClosedOnly = model.ClosedOnly;
                    schedule.OverrideMessage = model.OverrideMessage;
                    _repository.SaveChanges();
                    if (schedule.Profiles.Any())
                    {
                        UpdateSchedulesSync(schedule);
                    }
                    return Json(new {success = true});
                }
            }
            catch (Exception)
            {
            }

            return PartialView("_EditSchedule", model);
        }

        void UpdateSchedulesSync(Schedule schedule)
        {
            foreach (var profile in schedule.Profiles.ToList())
            {
                if (!ScheduleManager.ProfilesToReload.Contains(profile))
                {
                    ScheduleManager.ProfilesToReload.Add(profile);
                }
            }
        }

        [HttpGet]
        public ActionResult CreateTemplate()
        {
            const string defaultName = "Template";
            var templateName = "";
            var cnt = 1;
            var distinct = false;
            while (!distinct)
            {
                templateName = defaultName + cnt;
                var existing = _repository.Templates.FirstOrDefault(t => t.Title == templateName);
                if (existing != null)
                {
                    cnt++;
                }
                else
                {
                    distinct = true;
                }
            }
            var template = new Template
            {
                HeaderIcons = true,
                HeaderText = templateName,
                IncludeHeader = true,
                IncludePrint = false,
                IncludeTranscript = false,
                IncludeDisconnect = false,
                MessageArrows = true,
                SendIncludeIcon = true,
                SendButtonIcon = "/Images/Icons/paper-plane.png",
                ShowInitials = false,
                ShowTime = false,
                Title = templateName,
                HideHeader = false,
                HideSendButton = false,
                UseUnstyledHeaderIcons = false,
                CloseButtonIcon = "/Images/Close/x.png",
                DisconnectButtonIcon = "/Images/Close/x.png"
            };
            _repository.Templates.Add(template);
            _repository.SaveChanges();
            var model = TemplateDefaults();
            model.TemplateId = template.TemplateId;
            model.HeaderIcons = true;
            model.HeaderText = template.Title;
            model.IncludeHeader = true;
            model.IncludePrint = true;
            model.IncludeTranscript = false;
            model.IncludeDisconnect = false;
            model.MessageArrows = true;
            model.SendIncludeIcon = true;
            model.SendButtonIcon = "/Images/Icons/paper-plane.png";
            model.ShowInitials = false;
            model.ShowTime = false;
            model.HideHeader = false;
            model.HideSendButton = false;
            model.Title = template.Title;
            model.CloseButtonIcon = "/Images/Close/x.png";
            model.DisconnectButtonIcon = "/Images/Close/x.png";
            model.UseUnstyledHeaderIcons = false;
            return PartialView("_CreateTemplate", model);
        }

        public CreateTemplateViewModel TemplateDefaults()
        {
            var model = new CreateTemplateViewModel
            {
                Logos = GetLogos(),
                SendIcons = Directory.EnumerateFiles(Server.MapPath("~/Images/Icons")).Select(fn => "/Images/Icons/" + Path.GetFileName(fn)).ToList(),
                CloseButtonIcons = Directory.EnumerateFiles(Server.MapPath("~/Images/Close")).Select(fn => "/Images/Close/" + Path.GetFileName(fn)).ToList(),
                HeaderButtonIcons = Directory.EnumerateFiles(Server.MapPath("~/Images/Header")).Select(fn => "/Images/Header/" + Path.GetFileName(fn)).ToList(),
                BackgroundColor = "#333",
                HeaderFontColor = "#fff",
                AgentBackgroundColor = "#fff",
                ImageBackgroundColor = "#808080",
                ServerBackgroundColor = "#a7a7a7",
                VisitorBackgroundColor = "#22a7f0",
                AgentFontColor = "#22a7f0",
                AgentLinkColor = "#22a7f0",
                InitialsFontColor = "#fff",
                ServerFontColor = "#fff",
                VisitorFontColor = "#fff",
                VisitorLinkColor = "#fff",
                AgentBorderColor = "#22a7f0",
                VisitorBorderColor = "#22a7f0",
                VisitorNameColor = "#333",
                AgentNameColor = "#333",
                ServerSeparatorColor = "#757575",
                ImageBorderColor = "#333",
                AgentTypingFontColor = "#fff",
                AgentTypingBackgroundColor = "rgba(211, 211, 211, 0.75)",
                RoundImages = true,
                RoundHeader = false,
                PreviewHeight = 360,
                PreviewWidth = 360,
            };
            return model;
        }

        [HttpGet]
        public ActionResult EditTemplate(int templateId)
        {
            try
            {
                var template = _repository.Templates.Find(templateId);
                if (template != null)
                {
                    var model = TemplateDefaults();
                    if (!String.IsNullOrWhiteSpace(template.CustomCss))
                    {
                        model.CustomCss = template.CustomCss;
                        var dataFile = Server.MapPath(template.CustomCss);
                        try
                        {
                            if (System.IO.File.Exists(dataFile))
                            {
                                using (var reader = new StreamReader(dataFile))
                                {
                                    string contents = reader.ReadToEnd();
                                    model.AgentBackgroundColor = GetValue(contents, ".chat-received .chat-msg-text", "background-color:", model.AgentBackgroundColor);
                                    model.AgentBorderColor = GetValue(contents, ".chat-received .chat-msg-text", "border:", model.AgentBorderColor).Split(' ').Last();
                                    model.AgentFontColor = GetValue(contents, ".chat-received .chat-msg-text", " color:", model.AgentFontColor);
                                    model.AgentLinkColor = GetValue(contents, ".chat-received .chat-msg-text a", " color:", model.AgentLinkColor);
                                    model.AgentNameColor = GetValue(contents, ".chat-received .chat-msg-icon .chat-display-name", " color:", model.AgentNameColor);
                                    model.AgentTypingBackgroundColor = GetValue(contents, "#wcb-chat-agent-typing", "background-color:", model.AgentTypingBackgroundColor);
                                    model.AgentTypingFontColor = GetValue(contents, "#wcb-chat-agent-typing", " color:", model.AgentTypingFontColor);
                                    model.BackgroundColor = GetValue(contents, "#wcb-body", "background-color:", model.BackgroundColor);
                                    model.HeaderFontColor = GetValue(contents, "#wcb-chat-header", "color:", model.HeaderFontColor);
                                    model.ImageBackgroundColor = GetValue(contents, ".chat-msg-initial-wrapper", "background-color:", model.ImageBackgroundColor);
                                    model.ImageBorderColor = GetValue(contents, ".chat-msg-initial-wrapper", "border:", model.ImageBorderColor).Split(' ').Last();
                                    model.InitialsFontColor = GetValue(contents, ".chat-msg-initials", " color:", model.InitialsFontColor);
                                    model.ServerBackgroundColor = GetValue(contents, ".chat-system-message", "background-color:", model.ServerBackgroundColor);
                                    model.ServerFontColor = GetValue(contents, ".chat-system-message", " color:", model.ServerFontColor);
                                    model.ServerSeparatorColor = GetValue(contents, ".chat-system-message", "border-bottom:", model.ServerSeparatorColor).Split(' ').Last();
                                    model.VisitorBackgroundColor = GetValue(contents, ".chat-sent .chat-msg-text", "background-color:", model.VisitorBackgroundColor);
                                    model.VisitorBorderColor = GetValue(contents, ".chat-sent .chat-msg-text", "border:", model.VisitorBorderColor).Split(' ').Last();
                                    model.VisitorFontColor = GetValue(contents, ".chat-sent .chat-msg-text", " color:", model.VisitorFontColor);
                                    model.VisitorLinkColor = GetValue(contents, ".chat-sent .chat-msg-text a", " color:", model.VisitorLinkColor);
                                    model.VisitorNameColor = GetValue(contents, ".chat-sent .chat-msg-icon .chat-display-name", " color:", model.VisitorNameColor);
                                    model.RoundImages = GetValue(contents, ".chat-msg-initial-wrapper", "border-radius:", "50%") == "50%";
                                    model.RoundHeader = GetValue(contents, "#wcb-body", "border-radius:", "0") != "0";
                                    model.SimplifyTemplate = HasSimplified(contents);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            return Json(new {success = false, error = e.Message}, JsonRequestBehavior.AllowGet);
                        }
                    }
                    model.TemplateId = template.TemplateId;
                    model.HeaderLogoPath = template.HeaderLogoPath;
                    model.HeaderIcons = template.HeaderIcons;
                    model.HeaderText = template.HeaderText;
                    model.IncludeHeader = template.IncludeHeader;
                    model.IncludePrint = template.IncludePrint;
                    model.IncludeTranscript = template.IncludeTranscript;
                    model.IncludeDisconnect = template.IncludeDisconnect;
                    model.MessageArrows = template.MessageArrows;
                    model.SendIncludeIcon = template.SendIncludeIcon;
                    model.SendButtonIcon = template.SendButtonIcon;
                    model.ShowInitials = template.ShowInitials;
                    model.ShowTime = template.ShowTime;
                    model.Title = template.Title;
                    model.PlaceholderText = template.PlaceholderText;
                    model.HideHeader = template.HideHeader;
                    model.HideSendButton = template.HideSendButton;
                    model.CloseButtonIcon = template.CloseButtonIcon;
                    model.DisconnectButtonIcon = template.DisconnectButtonIcon;
                    model.PrintButtonIcon = template.PrintButtonIcon;
                    model.EmailButtonIcon = template.EmailButtonIcon;
                    model.UseUnstyledHeaderIcons = template.UseUnstyledHeaderIcons;
                    model.PreviewHeight = 360;
                    model.PreviewWidth = 360;
                    return PartialView("_CreateTemplate", model);
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateTemplate(CreateTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var template = _repository.Templates.Find(model.TemplateId);
                if (template != null)
                {
                    template.Title = model.Title;
                    template.CustomCss = model.CustomCss;
                    template.HeaderIcons = model.HeaderIcons;
                    template.HeaderLogoPath = model.HeaderLogoPath;
                    template.HeaderText = model.HeaderText;
                    template.IncludeHeader = model.IncludeHeader;
                    template.IncludePrint = model.IncludePrint;
                    template.IncludeTranscript = model.IncludeTranscript;
                    template.IncludeDisconnect = model.IncludeDisconnect;
                    template.MessageArrows = model.MessageArrows;
                    template.PlaceholderText = model.PlaceholderText;
                    template.SendButtonIcon = model.SendButtonIcon;
                    template.SendIncludeIcon = model.SendIncludeIcon;
                    template.ShowInitials = model.ShowInitials;
                    template.ShowTime = model.ShowTime;
                    template.HideHeader = model.HideHeader;
                    template.HideSendButton = model.HideSendButton;
                    template.CloseButtonIcon = model.CloseButtonIcon;
                    template.DisconnectButtonIcon = model.DisconnectButtonIcon;
                    template.PrintButtonIcon = model.PrintButtonIcon;
                    template.EmailButtonIcon = model.EmailButtonIcon;
                    template.UseUnstyledHeaderIcons = model.UseUnstyledHeaderIcons;
                    _repository.SaveChanges();
                    //return Json(new { success = true });
                }
            }
            model.Logos = GetLogos();
            model.SendIcons =
                Directory.EnumerateFiles(Server.MapPath("~/Images/Icons"))
                    .Select(fn => "/Images/Icons/" + Path.GetFileName(fn))
                    .ToList();
            model.CloseButtonIcons =
                Directory.EnumerateFiles(Server.MapPath("~/Images/Close"))
                    .Select(fn => "/Images/Close/" + Path.GetFileName(fn))
                    .ToList();
            model.HeaderButtonIcons =
                Directory.EnumerateFiles(Server.MapPath("~/Images/Header"))
                    .Select(fn => "/Images/Header/" + Path.GetFileName(fn))
                    .ToList();
            return PartialView("_CreateTemplate", model);
        }

        [HttpPost]
        public JsonResult DeleteTemplate(int templateId)
        {
            try
            {
                var template = _repository.Templates.Find(templateId);
                if (template != null)
                {
                    _repository.Templates.Remove(template);
                    _repository.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult ReloadTemplates()
        {
            var model = new TemplateSettingsViewModel
            {
                Templates = _repository.Templates.ToList()
            };
            return PartialView("_Templates", model);
        }

        [HttpGet]
        public ActionResult CreateWidget()
        {
            const string defaultName = "Widget";
            var widgetName = "";
            var cnt = 1;
            var distinct = false;
            while (!distinct)
            {
                widgetName = defaultName + cnt;
                var existing = _repository.Widgets.FirstOrDefault(t => t.Name.ToLower() == widgetName.ToLower());
                if (existing != null)
                {
                    cnt++;
                }
                else
                {
                    distinct = true;
                }
            }
            var widget = new Widget
            {
                Name = widgetName,
                IsActive = true,
                RecycleTime = 180,
                StartTime = 0,
                CheckForAgents = true,
                RequiredAgentsAvailable = 1,
                MaxEstimatedWaitTime = 0,
                ShowOnMobile = true,
                ShowTooltipOnMobile = true,
                MobileWidth = 767,
                UseIframe = false,
                PopOverlay = false,
                UseIcon = false,
                IconPath = "",
                IconWidth = 50,
                LaunchText = "Chat",
                Position = "bottom",
                Rounded = true,
                Vertical = false,
                Background = "#333333",
                TextColor = "#ffffff",
                PlaceHolderBackground = "#333333",
                ShowLoader = true,
                Height = 300,
                Width = 300,
                OffsetX = 50,
                OffsetY = 0,
                TooltipText = "Chat with us",
                TooltipColor = "#333333",
                ShowTooltip = true,
                ShowTooltipAtStart = 0,
                LaunchInNewWindow = false,
                IsSecondaryStyle = false
            };
            _repository.Widgets.Add(widget);
            _repository.SaveChanges();
            var model = WidgetDefaults();
            model.WidgetId = widget.WidgetId;
            model.Name = widgetName;
            return PartialView("_CreateWidget", model);
        }

        public CreateWidgetViewModel WidgetDefaults()
        {
            var forms = _repository.Forms.Select(f => new SelectListItem
            {
                Text = f.FormName,
                Value = f.FormId.ToString(),
                Selected = false
            }).ToList();
            forms.Add(new SelectListItem
            {
                Text = "None",
                Value = (-1).ToString(),
                Selected = true
            });
            var model = new CreateWidgetViewModel
            {
                IsActive = true,
                RecycleTime = 180,
                StartTime = 0,
                CheckForAgents = true,
                RequiredAgentsAvailable = 1,
                MaxEstimatedWaitTime = 0,
                ShowOnMobile = true,
                ShowTooltipOnMobile = true,
                MobileWidth = 767,
                UseIframe = false,
                PopOverlay = false,
                UseIcon = false,
                IconPath = "",
                ResumeIconPath = "",
                UnavailableIconPath = "",
                IconWidth = 50,
                LaunchText = "Chat",
                ResumeLaunchText = "Chat",
                UnavailableLaunchText = "",
                Position = "bottom",
                Rounded = true,
                Vertical = false,
                Background = "#333333",
                TextColor = "#ffffff",
                PlaceHolderBackground = "#333333",
                ShowLoader = true,
                Height = 325,
                Width = 325,
                OffsetX = 50,
                OffsetY = 0,
                TooltipText = "Chat with us",
                UnavailableTooltipText = "",
                ResumeTooltipText = "Chat with us",
                TooltipColor = "#333333",
                ShowTooltip = true,
                ShowTooltipAtStart = 0,
                LaunchInNewWindow = false,
                Icons = GetLaunchIcons(),
                Forms = forms,
                UnavailableForms = forms,
                FormId = -1,
                UnavailableFormId = -1,
                ShowUnavailableIfOpenNoAgents = false,
                IsSecondaryStyle = false
            };
            return model;
        }

        [HttpGet]
        public ActionResult EditWidget(int widgetId)
        {
            var widget = _repository.Widgets.Find(widgetId);
            if (widget != null)
            {
                var model = new CreateWidgetViewModel
                {
                    WidgetId = widget.WidgetId,
                    Name = widget.Name,
                    IsActive = widget.IsActive,
                    RecycleTime = widget.RecycleTime,
                    StartTime = widget.StartTime,
                    CheckForAgents = widget.CheckForAgents,
                    RequiredAgentsAvailable = widget.RequiredAgentsAvailable,
                    MaxEstimatedWaitTime = widget.MaxEstimatedWaitTime,
                    ShowLoader = widget.ShowLoader,
                    ShowOnMobile = widget.ShowOnMobile,
                    ShowTooltipOnMobile = widget.ShowTooltipOnMobile,
                    MobileWidth = widget.MobileWidth,
                    UseIframe = widget.UseIframe,
                    UseIcon = widget.UseIcon,
                    IconPath = widget.IconPath,
                    IconWidth = widget.IconWidth,
                    ResumeIconPath = widget.ResumeIconPath,
                    UnavailableIconPath = widget.UnavailableIconPath,
                    LaunchText = widget.LaunchText,
                    ResumeLaunchText = widget.ResumeLaunchText,
                    UnavailableLaunchText = widget.UnavailableLaunchText,
                    Position = widget.Position,
                    Rounded = widget.Rounded,
                    Vertical = widget.Vertical,
                    Background = widget.Background,
                    TextColor = widget.TextColor,
                    PlaceHolderBackground = widget.PlaceHolderBackground,
                    Height = widget.Height,
                    Width = widget.Width,
                    OffsetX = widget.OffsetX,
                    OffsetY = widget.OffsetY,
                    TooltipColor = widget.TooltipColor,
                    TooltipText = widget.TooltipText,
                    ResumeTooltipText = widget.ResumeTooltipText,
                    UnavailableTooltipText = widget.UnavailableTooltipText,
                    ShowTooltip = widget.ShowTooltip,
                    ShowTooltipAtStart = widget.ShowTooltipAtStart,
                    LaunchInNewWindow = widget.LaunchInNewWindow,
                    PopOverlay = widget.PopOverlay,
                    FormId = widget.Form != null ? widget.Form.FormId : -1,
                    UnavailableFormId = widget.UnavailableForm != null ? widget.UnavailableForm.FormId : -1,
                    IsSecondaryStyle = widget.IsSecondaryStyle,
                    ShowUnavailableIfOpenNoAgents = widget.ShowUnavailableIfOpenNoAgents
                };
                var forms = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "None",
                        Value = (-1).ToString(),
                        Selected = widget.Form == null
                    }
                };
                var unavailableForms = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "None",
                        Value = (-1).ToString(),
                        Selected = widget.Form == null
                    }
                };
                if (_repository.Forms.Any())
                {
                    foreach (var form in _repository.Forms)
                    {
                        var item = new SelectListItem
                        {
                            Text = form.FormName,
                            Value = form.FormId.ToString(),
                            Selected = widget.Form != null && form.FormId == widget.Form.FormId
                        };
                        forms.Add(item);
                        var i = new SelectListItem
                        {
                            Text = form.FormName,
                            Value = form.FormId.ToString(),
                            Selected = widget.UnavailableForm != null && form.FormId == widget.UnavailableForm.FormId
                        };
                        unavailableForms.Add(i);
                    }
                }
                model.Icons = GetLaunchIcons();
                model.Forms = forms;
                model.UnavailableForms = unavailableForms;
                return PartialView("_CreateWidget", model);
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult UpdateWidget(CreateWidgetViewModel model)
        {
            var widget = _repository.Widgets.Find(model.WidgetId);
            if (ModelState.IsValid)
            {
                if (widget != null)
                {
                    var form = _repository.Forms.Find(model.FormId);
                    var unavailableForm = _repository.Forms.Find(model.UnavailableFormId);
                    widget.Name = model.Name;
                    widget.IsActive = model.IsActive;
                    widget.RecycleTime = model.RecycleTime;
                    widget.StartTime = model.StartTime;
                    widget.CheckForAgents = model.CheckForAgents;
                    widget.RequiredAgentsAvailable = model.RequiredAgentsAvailable;
                    widget.MaxEstimatedWaitTime = model.MaxEstimatedWaitTime;
                    widget.ShowLoader = model.ShowLoader;
                    widget.ShowOnMobile = model.ShowOnMobile;
                    widget.ShowTooltipOnMobile = model.ShowTooltipOnMobile;
                    widget.MobileWidth = model.MobileWidth;
                    widget.UseIframe = model.UseIframe;
                    widget.UseIcon = model.UseIcon;
                    widget.IconPath = model.IconPath;
                    widget.ResumeIconPath = model.ResumeIconPath;
                    widget.UnavailableIconPath = model.UnavailableIconPath;
                    widget.IconWidth = model.IconWidth;
                    widget.LaunchText = model.LaunchText;
                    widget.ResumeLaunchText = model.ResumeLaunchText;
                    widget.UnavailableLaunchText = model.UnavailableLaunchText;
                    widget.Position = model.Position;
                    widget.Rounded = model.Rounded;
                    widget.Vertical = model.Vertical;
                    widget.Background = model.Background;
                    widget.TextColor = model.TextColor;
                    widget.PlaceHolderBackground = model.PlaceHolderBackground;
                    widget.Height = model.Height;
                    widget.Width = model.Width;
                    widget.OffsetX = model.OffsetX;
                    widget.OffsetY = model.OffsetY;
                    widget.TooltipColor = model.TooltipColor;
                    widget.TooltipText = model.TooltipText;
                    widget.ResumeTooltipText = model.ResumeTooltipText;
                    widget.UnavailableTooltipText = model.UnavailableTooltipText;
                    widget.ShowTooltip = model.ShowTooltip;
                    widget.ShowTooltipAtStart = model.ShowTooltipAtStart;
                    widget.LaunchInNewWindow = model.LaunchInNewWindow;
                    widget.PopOverlay = model.PopOverlay;
                    widget.Form = form;
                    widget.UnavailableForm = unavailableForm;
                    widget.IsSecondaryStyle = model.IsSecondaryStyle;
                    widget.ShowUnavailableIfOpenNoAgents = model.ShowUnavailableIfOpenNoAgents;
                    _repository.SaveChanges();
                    if (form == null)
                    {
                        while (widget.Form != null)
                        {
                            widget.Form = null;
                        }
                        _repository.SaveChanges();
                    }
                    if (unavailableForm == null)
                    {
                        while (widget.UnavailableForm != null)
                        {
                            widget.UnavailableForm = null;
                        }
                        _repository.SaveChanges();
                    }
                }
            }
            model.Icons = GetLaunchIcons();
            var forms = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "None",
                    Value = (-1).ToString(),
                    Selected = widget != null && widget.Form == null
                }
            };
            var unavailableForms = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "None",
                    Value = (-1).ToString(),
                    Selected = widget != null && widget.Form == null
                }
            };
            if (_repository.Forms.Any())
            {
                foreach (var form in _repository.Forms)
                {
                    var item = new SelectListItem
                    {
                        Text = form.FormName,
                        Value = form.FormId.ToString(),
                        Selected = widget != null && (widget.Form != null && form.FormId == widget.Form.FormId)
                    };
                    forms.Add(item);
                    var i = new SelectListItem
                    {
                        Text = form.FormName,
                        Value = form.FormId.ToString(),
                        Selected = widget != null && (widget.UnavailableForm != null && form.FormId == widget.UnavailableForm.FormId)
                    };
                    unavailableForms.Add(i);
                }
            }
            model.FormId = widget != null && widget.Form != null ? widget.Form.FormId : -1;
            model.UnavailableFormId = widget != null && widget.UnavailableForm != null ? widget.UnavailableForm.FormId : -1;
            model.Forms = forms;
            model.UnavailableForms = unavailableForms;

            return PartialView("_CreateWidget", model);
        }

        [HttpPost]
        public JsonResult DeleteWidget(int widgetId)
        {
            try
            {
                var widget = _repository.Widgets.Find(widgetId);
                if (widget != null)
                {
                    widget.UnavailableForm = null;
                    widget.Form = null;
                    _repository.Widgets.Remove(widget);
                    _repository.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new { success = false });
        }

        //Forms
        [HttpGet]
        public ActionResult CreateForm()
        {
            const string defaultName = "Form";
            var formName = "";
            var cnt = 1;
            var distinct = false;
            while (!distinct)
            {
                formName = defaultName + cnt;
                var existing = _repository.Forms.FirstOrDefault(t => t.FormName == formName);
                if (existing != null)
                {
                    cnt++;
                }
                else
                {
                    distinct = true;
                }
            }
            var form = new Form
            {
                FormName = formName,
                Rounded = true,
                BackgroundColor = "#ffffff",
                LabelColor = "#333333",
                BorderColor = "#333333",
                ButtonColor = "#5cb85c",
                ButtonTextColor = "#ffffff",
                ButtonText = "Start Chat"
            };
            _repository.Forms.Add(form);
            _repository.SaveChanges();
            var model = FormDefaults();
            model.FormId = form.FormId;
            model.FormName = formName;
            return PartialView("_CreateForm", model);
        }

        public CreateFormViewModel FormDefaults()
        {
            var model = new CreateFormViewModel
            {
                Rounded = true,
                BackgroundColor = "#ffffff",
                LabelColor = "#333333",
                BorderColor = "#333333",
                ButtonColor = "#5cb85c",
                ButtonTextColor = "#ffffff",
                ButtonText = "Start Chat",
                ShowFormMessage = false,
                UseScheduleMessage = false,
                FieldTypes = new Collection<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = FieldType.Text.ToString(),
                        Value = FieldType.Text.ToString()
                    },
                    new SelectListItem
                    {
                        Text = FieldType.Password.ToString(),
                        Value = FieldType.Password.ToString()
                    },
                    new SelectListItem
                    {
                        Text = FieldType.Email.ToString(),
                        Value = FieldType.Email.ToString()
                    },
                    new SelectListItem
                    {
                        Text = FieldType.Select.ToString(),
                        Value = FieldType.Select.ToString()
                    },
                    new SelectListItem
                    {
                        Text = FieldType.Profiles.ToString(),
                        Value = FieldType.Profiles.ToString()
                    },
                    new SelectListItem
                    {
                        Text = FieldType.TextArea.ToString(),
                        Value = FieldType.TextArea.ToString()
                    }
                }
            };
            return model;
        }

        [HttpGet]
        public ActionResult EditForm(int formId)
        {
            var form = _repository.Forms.Find(formId);
            if (form != null)
            {
                var model = new CreateFormViewModel
                {
                    FormId = form.FormId,
                    FormName = form.FormName,
                    FormMessage = form.FormMessage,
                    ShowFormMessage = form.ShowFormMessage,
                    FormSubmittedMessage = form.FormSubmittedMessage,
                    UseScheduleMessage = form.UseScheduleMessage,
                    BackgroundColor = form.BackgroundColor,
                    BorderColor = form.BorderColor,
                    ButtonColor = form.ButtonColor,
                    ButtonText = form.ButtonText,
                    ButtonTextColor = form.ButtonTextColor,
                    FieldTypes = new Collection<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = FieldType.Text.ToString(),
                            Value = FieldType.Text.ToString()
                        },
                        new SelectListItem
                        {
                            Text = FieldType.Password.ToString(),
                            Value = FieldType.Password.ToString()
                        },
                        new SelectListItem
                        {
                            Text = FieldType.Email.ToString(),
                            Value = FieldType.Email.ToString()
                        },
                        new SelectListItem
                        {
                            Text = FieldType.Select.ToString(),
                            Value = FieldType.Select.ToString()
                        },
                        new SelectListItem
                        {
                            Text = FieldType.Profiles.ToString(),
                            Value = FieldType.Profiles.ToString()
                        },
                        new SelectListItem
                        {
                            Text = FieldType.TextArea.ToString(),
                            Value = FieldType.TextArea.ToString()
                        }
                    },
                    FormFields = form.FormFields,
                    LabelColor = form.LabelColor,
                    Rounded = form.Rounded
                };

                return PartialView("_CreateForm", model);
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public JsonResult DeleteForm(int formId)
        {
            try
            {
                var form = _repository.Forms.Find(formId);
                if (form != null)
                {
                    var widgets = _repository.Widgets.Where(w => w.Form.FormId == form.FormId);
                    foreach (var widget in widgets)
                    {
                        widget.Form = null;
                    }
                    foreach (var formField in form.FormFields.ToList())
                    {
                        if (formField.FieldType == FieldType.Profiles || formField.FieldType == FieldType.Select)
                        {
                            var options = formField.SelectOptions.ToList();
                            formField.SelectOptions.Clear();
                            foreach (var formFieldSelectOption in options)
                            {
                                _repository.FormOptions.Remove(formFieldSelectOption);
                            }
                        }
                        form.FormFields.Remove(formField);
                        _repository.FormFields.Remove(formField);
                    }
                    _repository.Forms.Remove(form);
                    _repository.SaveChanges();
                    return Json(new {success = true});
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.ToString() + "||" + e.Message });
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult UpdateForm(CreateFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var form = _repository.Forms.Find(model.FormId);
                if (form != null)
                {
                    form.FormName = model.FormName;
                    form.BackgroundColor = model.BackgroundColor;
                    form.ButtonColor = model.ButtonColor;
                    form.ButtonText = model.ButtonText;
                    form.ButtonTextColor = model.ButtonTextColor;
                    form.LabelColor = model.LabelColor;
                    form.BorderColor = model.BorderColor;
                    form.Rounded = model.Rounded;
                    form.FormMessage = model.FormMessage;
                    form.FormSubmittedMessage = model.FormSubmittedMessage;
                    form.ShowFormMessage = model.ShowFormMessage;
                    form.UseScheduleMessage = model.UseScheduleMessage;
                    _repository.SaveChanges();
                }
            }
            model.FieldTypes = new Collection<SelectListItem>
            {
                new SelectListItem
                {
                    Text = FieldType.Text.ToString(),
                    Value = FieldType.Text.ToString()
                },
                new SelectListItem
                {
                    Text = FieldType.Password.ToString(),
                    Value = FieldType.Password.ToString()
                },
                new SelectListItem
                {
                    Text = FieldType.Email.ToString(),
                    Value = FieldType.Email.ToString()
                },
                new SelectListItem
                {
                    Text = FieldType.Select.ToString(),
                    Value = FieldType.Select.ToString()
                },
                new SelectListItem
                {
                    Text = FieldType.Profiles.ToString(),
                    Value = FieldType.Profiles.ToString()
                },
                new SelectListItem
                {
                    Text = FieldType.TextArea.ToString(),
                    Value = FieldType.TextArea.ToString()
                }
            };
            return PartialView("_CreateForm", model);
        }

        [HttpGet]
        public ActionResult GetFormFields(int formId)
        {
            var form = _repository.Forms.Find(formId);
            if (form != null)
            {
                var formFields = form.FormFields.OrderBy(f => f.Position);
                return PartialView("_FormFields", formFields.ToList());
            }
            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateFormField(int formId, string fieldName, string customClasses, string label, string placeholder, FieldType fieldType, bool isUserField, bool isCustomInfo, bool isAttribute, bool isRequired, bool isPhone, int maxLength)
        {
            try
            {
                var form = _repository.Forms.Find(formId);
                if (form != null)
                {
                    var pos = 1;
                    if (form.FormFields.Any())
                    {
                        pos = form.FormFields.Max(f => f.Position) + 1;
                    }
                    var formField = new FormField
                    {
                        Label = label,
                        PlaceHolder = placeholder,
                        FieldType = fieldType,
                        IsUserField = isUserField,
                        SendAsAttribute = isAttribute,
                        AppendToCustomInfo = isCustomInfo,
                        Position = pos,
                        Name = fieldName,
                        CustomClasses = customClasses,
                        IsRequired = isRequired,
                        IsPhoneNumber = isPhone,
                        MaxLength = maxLength
                    };
                    _repository.FormFields.Add(formField);
                    form.FormFields.Add(formField);
                    _repository.SaveChanges();
                    return Json(new {success = true});
                }
            }
            catch (Exception)
            {
            }
            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFormField(int fieldId)
        {
            try
            {
                var formField = _repository.FormFields.Find(fieldId);
                if (formField != null)
                {
                    return PartialView("_EditField", formField);
                }
            }
            catch (Exception)
            {
            }
            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateFormField(int formFieldId, string customClasses, string label, string placeHolder, bool isUserField, bool appendToCustomInfo, bool sendAsAttribute, bool isRequired, bool isPhoneNumber, int maxLength)
        {
            try
            {
                var formField = _repository.FormFields.Find(formFieldId);
                if (formField != null)
                {
                    formField.CustomClasses = customClasses;
                    formField.Label = label;
                    formField.PlaceHolder = placeHolder;
                    formField.IsUserField = isUserField;
                    formField.IsRequired = isRequired;
                    formField.IsPhoneNumber = isPhoneNumber;
                    formField.AppendToCustomInfo = appendToCustomInfo;
                    formField.SendAsAttribute = sendAsAttribute;
                    formField.MaxLength = maxLength;
                    _repository.SaveChanges();
                    return Json(new {success = true});
                }
            }
            catch (Exception)
            {
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public JsonResult DeleteFormField(int id, int formId)
        {
            try
            {
                var formField = _repository.FormFields.Find(id);
                var form = _repository.Forms.Find(formId);
                if (formField != null && form != null)
                {
                    if ((formField.FieldType == FieldType.Profiles || formField.FieldType == FieldType.Select) && formField.SelectOptions != null && formField.SelectOptions.Any())
                    {
                        var options = formField.SelectOptions.ToList();
                        formField.SelectOptions.Clear();
                        foreach (var formFieldSelectOption in options)
                        {
                            _repository.FormOptions.Remove(formFieldSelectOption);
                        }
                    }
                    form.FormFields.Remove(formField);
                    _repository.FormFields.Remove(formField);
                    _repository.SaveChanges();
                    return Json(new {success = true});
                }
            }
            catch (Exception)
            {
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public JsonResult RaiseFormField(int id, int formId)
        {
            try
            {
                var formField = _repository.FormFields.Find(id);
                var form = _repository.Forms.Find(formId);
                if (formField != null && form != null)
                {
                    var currentPos = formField.Position;
                    var newPos = formField.Position - 1;
                    var fieldAbove = form.FormFields.FirstOrDefault(f => f.Position == newPos);
                    if (fieldAbove != null)
                    {
                        fieldAbove.Position = currentPos;
                        formField.Position = newPos;
                        _repository.SaveChanges();
                        return Json(new {success = true});
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(new {success = false});
        }

        [HttpGet]
        public ActionResult GetFieldSelectOptions(int fieldId)
        {
            try
            {
                var formField = _repository.FormFields.Find(fieldId);
                if (formField != null)
                {
                    var profiles = new List<string>();
                    var hasProfiles = formField.FieldType == FieldType.Profiles;
                    if (hasProfiles)
                    {
                        var profileNames = _repository.Profiles.Select(i => i.Name);
                        profiles.AddRange(profileNames);
                    }
                    var model = new SelectOptionsViewModel
                    {
                        FieldId = formField.FormFieldId,
                        FormOptions = formField.SelectOptions,
                        IsProfileList = hasProfiles,
                        Profiles = profiles
                    };
                    return PartialView("_SelectOptions", model);
                }
            }
            catch (Exception)
            {
            }
            return Json(new {success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSelectOption(int fieldId, string text, string value)
        {
            try
            {
                var formField = _repository.FormFields.Find(fieldId);
                if (formField != null)
                {
                    var formOption = new FormOption
                    {
                        Text = text,
                        Value = value
                    };

                    if (!formField.SelectOptions.Any())
                    {
                        formOption.IsDefault = true;
                    }

                    if (formField.FieldType == FieldType.Profiles)
                    {
                        var profile = _repository.Profiles.FirstOrDefault(i => i.Name == value);
                        if (profile != null)
                        {
                            formOption.Profile = profile;
                        }
                    }
                    formField.SelectOptions.Add(formOption);
                    _repository.SaveChanges();

                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult RemoveSelectOption(int fieldId, int fieldOption)
        {
            try
            {
                var formOption = _repository.FormOptions.Find(fieldOption);
                var formField = _repository.FormFields.Find(fieldId);
                if (formField != null && formOption != null)
                {
                    formField.SelectOptions.Remove(formOption);
                    _repository.FormOptions.Remove(formOption);
                    _repository.SaveChanges();

                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult SetDefaultSelectOption(int fieldId, int fieldOption)
        {
            try
            {
                var formOption = _repository.FormOptions.Find(fieldOption);
                var formField = _repository.FormFields.Find(fieldId);
                if (formField != null && formOption != null)
                {
                    var defaultOption = formField.SelectOptions.FirstOrDefault(i => i.IsDefault);
                    if (defaultOption != null)
                    {
                        defaultOption.IsDefault = false;
                    }
                    formOption.IsDefault = true;
                    _repository.SaveChanges();

                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult UnavailableOptions(WidgetSettingsViewModel model)
        {
            try
            {
                var confirmNameInVisitorMessages = _repository.Settings.FirstOrDefault(s => s.Key == "ConfirmNameInVisitorMessages");
                var useProfileWorkgroupForVisitorMessages = _repository.Settings.FirstOrDefault(s => s.Key == "UseProfileWorkgroupForVisitorMessages");
                var useCallbackForVisitorMessages = _repository.Settings.FirstOrDefault(s => s.Key == "UseCallbackForVisitorMessages");
                var defaultWorkgroupForVisitorMessages = _repository.Settings.FirstOrDefault(s => s.Key == "DefaultWorkgroupForVisitorMessages");

                if (confirmNameInVisitorMessages != null && useCallbackForVisitorMessages != null && useProfileWorkgroupForVisitorMessages != null && defaultWorkgroupForVisitorMessages != null)
                {
                    confirmNameInVisitorMessages.Value = model.ConfirmNameInVisitorMessages.ToString();
                    useCallbackForVisitorMessages.Value = model.UseCallbackForVisitorMessages.ToString();
                    useProfileWorkgroupForVisitorMessages.Value = model.UseProfileWorkgroupForVisitorMessages.ToString();
                    defaultWorkgroupForVisitorMessages.Value = model.DefaultWorkgroupForVisitorMessages;
                }
                _repository.SaveChanges();

                ChatController.ConfirmNameInVisitorMessages = model.ConfirmNameInVisitorMessages;
                ChatController.UseProfileWorkgroupForVisitorMessages = model.UseProfileWorkgroupForVisitorMessages;
                ChatController.UseCallbackForVisitorMessages = model.UseCallbackForVisitorMessages;
                ChatController.DefaultWorkgroupForVisitorMessages = model.DefaultWorkgroupForVisitorMessages;
                return Json(new { success = true });
            }
            catch (Exception)
            {
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult ReloadWidgets()
        {
            var model = new WidgetSettingsViewModel
            {
                Widgets = _repository.Widgets.ToList(),
                Icons = GetLaunchIcons(),
                Forms = _repository.Forms.ToList(),
                UseCallbackForVisitorMessages = ChatController.UseCallbackForVMs(),
                UseProfileWorkgroupForVisitorMessages = ChatController.UseProfileWorkgroupForVMs(),
                DefaultWorkgroupForVisitorMessages = ChatController.GetDefaultWorkgroupForVisitorMessages(),
                ConfirmNameInVisitorMessages = ChatController.ConfirmNameInVisitorMessagesVoicemail()
            };
            return PartialView("_Widgets", model);
        }

        [HttpGet]
        public ActionResult GetWidgetPreview(int widgetId)
        {
            try
            {
                var widget = _repository.Widgets.Find(widgetId);
                if (widget != null)
                {
                    var model = new DynamicChatHtmlViewModel
                    {
                        PopOverlay = widget.PopOverlay,
                        Background = widget.Background,
                        TextColor = widget.TextColor,
                        Height = widget.Height,
                        Width = widget.Width,
                        UseIcon = widget.UseIcon,
                        IconPath = widget.IconPath,
                        IconWidth = widget.IconWidth,
                        LaunchText = widget.LaunchText,
                        PlaceHolderBackground = widget.PlaceHolderBackground,
                        OffsetX = widget.OffsetX,
                        OffsetY = widget.OffsetY,
                        Position = widget.Position,
                        ShowLoader = widget.ShowLoader,
                        Rounded = widget.Rounded,
                        Vertical = widget.Vertical,
                        ShowTooltip = widget.ShowTooltip,
                        TooltipColor = widget.TooltipColor,
                        TooltipText = widget.TooltipText,
                        ResumeIconPath = widget.ResumeIconPath,
                        ResumeLaunchText = widget.ResumeLaunchText,
                        ResumeTooltipText = widget.ResumeTooltipText,
                        UnavailableTooltipText = widget.UnavailableTooltipText,
                        UnavailableIconPath = widget.UnavailableIconPath,
                        UnavailableLaunchText = widget.UnavailableLaunchText,
                        Domain = ""
                    };
                    return PartialView(widget.IsSecondaryStyle ? "_DynamicHtml2" : "_DynamicHtml", model);
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new { success = false },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddLaunchIcon()
        {
            var message = "";
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/Launch"), fileName);
                        file.SaveAs(path);
                        return Json(new { success = true, imgUrl = "/Images/Launch/" + fileName });
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return Json(new { success = false });
        }

        private ICollection<string> GetLaunchIcons()
        {
            try
            {
                return Directory.EnumerateFiles(Server.MapPath("~/Images/Launch")).Select(fn => "/Images/Launch/" + Path.GetFileName(fn)).ToList();
            }
            catch (Exception)
            {
                return new Collection<string>();
            }
        }

        public JsonResult GenerateCss(CreateTemplateViewModel model)
        {
            try
            {
                var text = RenderRazorViewToString("_CustomStyles", model);
                var folderPath = Server.MapPath("~/Styles");
                if (folderPath != null)
                {
                    var exists = Directory.Exists(folderPath);
                    if (!exists)
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }
                var path = "/Styles/template-" + model.TemplateId + ".css";
                var dataFile = Server.MapPath(path);
                if (dataFile != null)
                {
                    if (System.IO.File.Exists(dataFile))
                    {
                        using (var reader = new StreamReader(dataFile))
                        {
                            string contents = reader.ReadToEnd();
                            var custom = GetCustom(contents);
                            if (!String.IsNullOrWhiteSpace(custom))
                            {
                                text += Environment.NewLine;
                                text += Environment.NewLine;
                                text += custom;
                            }
                        }
                    }

                    using (var writer = new StreamWriter(dataFile, false))
                    {
                        writer.Write(text);
                    }
                    var template = _repository.Templates.Find(model.TemplateId);
                    if (template != null)
                    {
                        template.CustomCss = path;
                        _repository.SaveChanges();
                        return Json(new { success = true, cssPath = path });
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
            return Json(new { success = false });
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            var viewData = new ViewDataDictionary(model);
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, viewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }


        public string GetValue(string source, string tag, string value, string defaultValue)
        {
            try
            {
                if (source.Contains(tag))
                {
                    int tagStart = source.IndexOf(tag, 0);
                    int valueStart = source.IndexOf(value, tagStart) + value.Length;
                    int valueEnd = source.IndexOf(";", valueStart);
                    var returnValue = source.Substring(valueStart, valueEnd-valueStart).Trim(';').Trim();//This is wrong
                    return returnValue;
                }
            }
            catch (Exception e)
            {
                //
            }
            return defaultValue;
        }

        public bool HasSimplified(string source)
        {
            return source.Contains("%SIMPLIFIED_TEMPLATE_SECTION%");
        }

        public string GetCustom(string source)
        {
            const string start = "/*%START_CUSTOM_SECTION%";
            const string end = "%END_CUSTOM_SECTION%*/";
            if (source.Contains(start))
            {
                int customStart = source.IndexOf(start, 0);
                int customEnd = source.IndexOf(end, customStart);
                return source.Substring(customStart, (customEnd + end.Length)-customStart);
            }
            return "";
        }
    }
}