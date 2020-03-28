using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.AspNet.SignalR;
using WebchatBuilder.DataModels;
using WebchatBuilder.Helpers;
using WebchatBuilder.Hubs;
using WebchatBuilder.Services;
using WebchatBuilder.ViewModels;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;
using WebGrease.Css.Extensions;
using System.Web.UI;

namespace WebchatBuilder.Controllers
{
    [AllowCrossDomain]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ChatController : Controller
    {
        private readonly Repository _repository = new Repository();
        public static bool? GatherIpAddress;
        public static string DefaultUserName;
        public static bool? OverrideAgentName;
        public static string DefaultAgentName;
        public static bool? AllowContinueChat;
        public static int? PausedChatTimeout;
        public static bool? EnableSendAndQueueChatsBeforeAgent;
        public static bool? AllowIframes;
        public static bool? DropInactiveChats;
        public static bool? ResetActivityTimeoutOnAgentMessage;
        public static int? InactiveChatTimeout;
        public static bool? BlockCicSystemMessages;
        public static bool? UseCustomSystemMessages;
        public static bool? KeepOpenOnDisconnectAndStartNew;
        public static bool? PassHistoryToNewAgentOnRestart;
        public static bool? ReloadUserHistoryOnNewChat;
        public static bool? ReloadUnansweredChatHistory;
        public static string CustomErrorText;
        public static string ConnectionLostText;
        public static bool? LoggingEnabled;
        public static bool? ConfirmNameInVisitorMessages;
        public static bool? UseProfileWorkgroupForVisitorMessages;
        public static string DefaultWorkgroupForVisitorMessages;
        public static bool? UseCallbackForVisitorMessages;
        //New way - old parameters should be refactored
        private static bool? _enableBrowserAlerts;
        private static bool? _enableAudioAlerts;
        private static bool? _showOptionsButton;
        private static bool? _enableHardDisconnect;
        private static bool? _showUserAgentInUserData;
        private static int? _transferTimeout;
        private static string _emailTranscriptSubject;
        private static string _closeButtonTitle;
        private static string _disconnectButtonTitle;
        private static bool? _keepQueuedChatsAlive;
        private static bool? _showCustomInfoOnReload;
        private static bool? _showCustomInfoOnLoad;
        private static bool? _enableGoogleAnalytics;
        private static string _googleAnalyticsTrackingId;
        private static bool? _useSimpleAvailabilityCheck;


        public static bool UseSimpleAvailabilityCheck
        {
            get
            {
                if (_useSimpleAvailabilityCheck == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "UseSimpleAvailabilityCheck");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "UseSimpleAvailabilityCheck",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _useSimpleAvailabilityCheck = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _useSimpleAvailabilityCheck = true;
                    }
                }
                return _useSimpleAvailabilityCheck.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "UseSimpleAvailabilityCheck");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "UseSimpleAvailabilityCheck",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _useSimpleAvailabilityCheck = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static string GoogleAnalyticsTrackingId
        {
            get
            {
                if (_googleAnalyticsTrackingId == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "GoogleAnalyticsTrackingId");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "GoogleAnalyticsTrackingId",
                                Value = ""
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _googleAnalyticsTrackingId = setting.Value;
                    }
                    catch (Exception)
                    {
                        _googleAnalyticsTrackingId = "";
                    }
                }
                return _googleAnalyticsTrackingId;
            }
            set
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "GoogleAnalyticsTrackingId");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "GoogleAnalyticsTrackingId",
                            Value = value
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = value;
                        repository.SaveChanges();
                    }
                    _googleAnalyticsTrackingId = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool EnableGoogleAnalytics
        {
            get
            {
                if (_enableGoogleAnalytics == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableGoogleAnalytics");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "EnableGoogleAnalytics",
                                Value = "true"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _enableGoogleAnalytics = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _enableGoogleAnalytics = true;
                    }
                }
                return _enableGoogleAnalytics.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableGoogleAnalytics");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "EnableGoogleAnalytics",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _enableGoogleAnalytics = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool ShowCustomInfoOnLoad
        {
            get
            {
                if (_showCustomInfoOnLoad == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowCustomInfoOnLoad");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "ShowCustomInfoOnLoad",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _showCustomInfoOnLoad = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _showCustomInfoOnLoad = true;
                    }
                }
                return _showCustomInfoOnLoad.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowCustomInfoOnLoad");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ShowCustomInfoOnLoad",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _showCustomInfoOnLoad = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool ShowCustomInfoOnReload
        {
            get
            {
                if (_showCustomInfoOnReload == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowCustomInfoOnReload");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "ShowCustomInfoOnReload",
                                Value = "true"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _showCustomInfoOnReload = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _showCustomInfoOnReload = true;
                    }
                }
                return _showCustomInfoOnReload.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowCustomInfoOnReload");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ShowCustomInfoOnReload",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _showCustomInfoOnReload = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool KeepQueuedChatsAlive
        {
            get
            {
                if (_keepQueuedChatsAlive == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "KeepQueuedChatsAlive");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "KeepQueuedChatsAlive",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _keepQueuedChatsAlive = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _keepQueuedChatsAlive = false;
                    }
                }
                return _keepQueuedChatsAlive.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "KeepQueuedChatsAlive");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "KeepQueuedChatsAlive",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _keepQueuedChatsAlive = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static string EmailTranscriptSubject
        {
            get
            {
                if (_emailTranscriptSubject == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "EmailTranscriptSubject");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "EmailTranscriptSubject",
                                Value = "Chat Transcript"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _emailTranscriptSubject = setting.Value;
                    }
                    catch (Exception)
                    {
                        _emailTranscriptSubject = "Chat Transcript";
                    }
                }
                return _emailTranscriptSubject;
            }
            set
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "EmailTranscriptSubject");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "EmailTranscriptSubject",
                            Value = value
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = value;
                        repository.SaveChanges();
                    }
                    _emailTranscriptSubject = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static string DisconnectButtonTitle
        {
            get
            {
                if (_disconnectButtonTitle == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "DisconnectButtonTitle");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "DisconnectButtonTitle",
                                Value = "End Chat"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _disconnectButtonTitle = setting.Value;
                    }
                    catch (Exception)
                    {
                        _disconnectButtonTitle = "End Chat";
                    }
                }
                return _disconnectButtonTitle;
            }
            set
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "DisconnectButtonTitle");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "DisconnectButtonTitle",
                            Value = value
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = value;
                        repository.SaveChanges();
                    }
                    _disconnectButtonTitle = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static string CloseButtonTitle
        {
            get
            {
                if (_closeButtonTitle == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "CloseButtonTitle");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "CloseButtonTitle",
                                Value = "Close Chat"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _closeButtonTitle = setting.Value;
                    }
                    catch (Exception)
                    {
                        _closeButtonTitle = "Close Chat";
                    }
                }
                return _closeButtonTitle;
            }
            set
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "CloseButtonTitle");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "CloseButtonTitle",
                            Value = value
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = value;
                        repository.SaveChanges();
                    }
                    _closeButtonTitle = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static int TransferTimeout
        {
            get
            {
                if (_transferTimeout == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "TransferTimeout");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "TransferTimeout",
                                Value = "30"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        int transferTimeout;
                        _transferTimeout = Int32.TryParse(setting.Value, out transferTimeout) ? transferTimeout : 30;
                    }
                    catch (Exception)
                    {
                        _transferTimeout = 30;
                    }
                }
                return _transferTimeout.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "TransferTimeout");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "TransferTimeout",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _transferTimeout = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool EnableHardDisconnect
        {
            get
            {
                if (_enableHardDisconnect == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableHardDisconnect");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "EnableHardDisconnect",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _enableHardDisconnect = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _enableHardDisconnect = false;
                    }
                }
                return _enableHardDisconnect.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableHardDisconnect");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "EnableHardDisconnect",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _enableHardDisconnect = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool ShowUserAgentInUserData
        {
            get
            {
                if (_showUserAgentInUserData == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowUserAgentInUserData");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "ShowUserAgentInUserData",
                                Value = "true"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _showUserAgentInUserData = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _showUserAgentInUserData = true;
                    }
                }
                return _showUserAgentInUserData.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowUserAgentInUserData");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ShowUserAgentInUserData",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _showUserAgentInUserData = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool ShowOptionsButton
        {
            get
            {
                if (_showOptionsButton == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowOptionsButton");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "ShowOptionsButton",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _showOptionsButton = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _showOptionsButton = false;
                    }
                }
                return _showOptionsButton.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ShowOptionsButton");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ShowOptionsButton",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _showOptionsButton = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool EnableBrowserAlerts
        {
            get
            {
                if (_enableBrowserAlerts == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableBrowserAlerts");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "EnableBrowserAlerts",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _enableBrowserAlerts = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _enableBrowserAlerts = false;
                    }
                }
                return _enableBrowserAlerts.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableBrowserAlerts");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "EnableBrowserAlerts",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _enableBrowserAlerts = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool EnableAudioAlerts
        {
            get
            {
                if (_enableAudioAlerts == null)
                {
                    try
                    {
                        var repository = new Repository();
                        var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableAudioAlerts");
                        if (setting == null)
                        {
                            setting = new Setting
                            {
                                Key = "EnableAudioAlerts",
                                Value = "false"
                            };
                            repository.Settings.Add(setting);
                            repository.SaveChanges();
                        }
                        _enableAudioAlerts = setting.Value.ToLower() == "true";
                    }
                    catch (Exception)
                    {
                        _enableAudioAlerts = false;
                    }
                }
                return _enableAudioAlerts.Value;
            }
            set
            {
                try
                {
                    var newValue = value.ToString().ToLower();
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableAudioAlerts");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "EnableAudioAlerts",
                            Value = newValue
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    else
                    {
                        setting.Value = newValue;
                        repository.SaveChanges();
                    }
                    _enableAudioAlerts = value;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool UseCallbackForVMs()
        {
            if (UseCallbackForVisitorMessages == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "UseCallbackForVisitorMessages");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "UseCallbackForVisitorMessages",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    UseCallbackForVisitorMessages = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    UseCallbackForVisitorMessages = false;
                }
            }
            return UseCallbackForVisitorMessages.Value;
        }

        private static string _wcbDomain;

        public static string GetDefaultWorkgroupForVisitorMessages()
        {
            if (String.IsNullOrEmpty(DefaultWorkgroupForVisitorMessages))
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "DefaultWorkgroupForVisitorMessages");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "DefaultWorkgroupForVisitorMessages",
                            Value = "wcbmessages"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    DefaultWorkgroupForVisitorMessages = setting.Value;
                }
                catch (Exception)
                {
                    DefaultWorkgroupForVisitorMessages = "wcbmessages";
                }
            }
            return DefaultWorkgroupForVisitorMessages;
        }

        public static bool UseProfileWorkgroupForVMs()
        {
            if (UseProfileWorkgroupForVisitorMessages == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "UseProfileWorkgroupForVisitorMessages");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "UseProfileWorkgroupForVisitorMessages",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    UseProfileWorkgroupForVisitorMessages = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    UseProfileWorkgroupForVisitorMessages = false;
                }
            }
            return UseProfileWorkgroupForVisitorMessages.Value;
        }

        public static bool ConfirmNameInVisitorMessagesVoicemail()
        {
            if (ConfirmNameInVisitorMessages == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ConfirmNameInVisitorMessages");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ConfirmNameInVisitorMessages",
                            Value = "true"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    ConfirmNameInVisitorMessages = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    ConfirmNameInVisitorMessages = false;
                }
            }
            return ConfirmNameInVisitorMessages.Value;
        }

        public static string GetConnectionLostText()
        {
            if (String.IsNullOrEmpty(ConnectionLostText))
            {
                try
                {
                    var repository = new Repository();
                    var connectionLostText = repository.Settings.FirstOrDefault(s => s.Key == "ConnectionLostText");
                    if (connectionLostText == null)
                    {
                        connectionLostText = new Setting
                        {
                            Key = "ConnectionLostText",
                            Value = "You are no longer connected to an agent."
                        };
                        repository.Settings.Add(connectionLostText);
                        repository.SaveChanges();
                    }
                    ConnectionLostText = connectionLostText.Value;
                }
                catch (Exception)
                {
                    ConnectionLostText = "You are no longer connected to an agent.";
                }
            }
            return ConnectionLostText;
        }

        public static bool EnableLogging()
        {
            if (LoggingEnabled == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "LoggingEnabled");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "LoggingEnabled",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    LoggingEnabled = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    LoggingEnabled = false;
                }
            }
            return LoggingEnabled.Value;
        }

        public static bool EnableReloadUnansweredChatHistory()
        {
            if (ReloadUnansweredChatHistory == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ReloadUnansweredChatHistory");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ReloadUnansweredChatHistory",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    ReloadUnansweredChatHistory = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    ReloadUnansweredChatHistory = false;
                }
            }
            return ReloadUnansweredChatHistory.Value;
        }

        public static bool EnableReloadUserHistoryOnNewChat()
        {
            if (ReloadUserHistoryOnNewChat == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ReloadUserHistoryOnNewChat");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ReloadUserHistoryOnNewChat",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    ReloadUserHistoryOnNewChat = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    ReloadUserHistoryOnNewChat = false;
                }
            }
            return ReloadUserHistoryOnNewChat.Value;
        }

        public static bool EnablePassHistoryToNewAgentOnRestart()
        {
            if (PassHistoryToNewAgentOnRestart == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "PassHistoryToNewAgentOnRestart");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "PassHistoryToNewAgentOnRestart",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    PassHistoryToNewAgentOnRestart = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    PassHistoryToNewAgentOnRestart = false;
                }
            }
            return PassHistoryToNewAgentOnRestart.Value;
        }

        public static bool EnableKeepOpenOnDisconnectAndStartNew()
        {
            if (KeepOpenOnDisconnectAndStartNew == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "KeepOpenOnDisconnectAndStartNew");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "KeepOpenOnDisconnectAndStartNew",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    KeepOpenOnDisconnectAndStartNew = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    KeepOpenOnDisconnectAndStartNew = false;
                }
            }
            return KeepOpenOnDisconnectAndStartNew.Value;
        }

        public static bool EnableCustomSystemMessages()
        {
            if (UseCustomSystemMessages == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "UseCustomSystemMessages");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "UseCustomSystemMessages",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    UseCustomSystemMessages = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    UseCustomSystemMessages = false;
                }
            }
            return UseCustomSystemMessages.Value;
        }

        public static bool BlockSystemMessages()
        {
            if (BlockCicSystemMessages == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "BlockCicSystemMessages");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "BlockCicSystemMessages",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    BlockCicSystemMessages = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    BlockCicSystemMessages = false;
                }
            }
            return BlockCicSystemMessages.Value;
        }

        public static bool EnableInactivityTimeout()
        {
            if (DropInactiveChats == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "DropInactiveChats");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "DropInactiveChats",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    DropInactiveChats = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    DropInactiveChats = false;
                }
            }
            return DropInactiveChats.Value;
        }

        public static bool EnableInactivityResetOnAgentMessage()
        {
            if (ResetActivityTimeoutOnAgentMessage == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ResetActivityTimeoutOnAgentMessage");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ResetActivityTimeoutOnAgentMessage",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    ResetActivityTimeoutOnAgentMessage = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    ResetActivityTimeoutOnAgentMessage = false;
                }
            }
            return ResetActivityTimeoutOnAgentMessage.Value;
        }

        public static int InactivityTimeout()
        {
            if (InactiveChatTimeout == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "InactiveChatTimeout");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "InactiveChatTimeout",
                            Value = "90"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    InactiveChatTimeout = Convert.ToInt32(setting.Value);
                }
                catch (Exception)
                {
                    InactiveChatTimeout = 90;
                }
            }
            return InactiveChatTimeout.Value;
        }

        public static bool EnableSendAndQueue()
        {
            if (EnableSendAndQueueChatsBeforeAgent == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "EnableSendAndQueueChatsBeforeAgent");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "EnableSendAndQueueChatsBeforeAgent",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    EnableSendAndQueueChatsBeforeAgent = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    EnableSendAndQueueChatsBeforeAgent = false;
                }
            }
            return EnableSendAndQueueChatsBeforeAgent.Value;
        }

        public static string WcbDomain
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_wcbDomain))
                {
                    _wcbDomain = ConfigurationManager.AppSettings["WcbDomain"];
                }
                return _wcbDomain;
            }
        }

        public static string GetUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                if (String.IsNullOrEmpty(DefaultUserName))
                {
                    try
                    {
                        var repository = new Repository();
                        var defaultUserSetting = repository.Settings.FirstOrDefault(s => s.Key == "DefaultUserName");
                        if (defaultUserSetting == null)
                        {
                            defaultUserSetting = new Setting
                            {
                                Key = "DefaultUserName",
                                Value = "ChatUser"
                            };
                            repository.Settings.Add(defaultUserSetting);
                            repository.SaveChanges();
                        }
                        DefaultUserName = defaultUserSetting.Value;
                    }
                    catch (Exception)
                    {
                        DefaultUserName = "ChatUser";
                    }
                }
                userName = DefaultUserName;
            }
            return userName;
        }

        public static string GetAgentName()
        {
            if (String.IsNullOrEmpty(DefaultAgentName))
            {
                try
                {
                    var repository = new Repository();
                    var defaultAgentSetting = repository.Settings.FirstOrDefault(s => s.Key == "DefaultAgentName");
                    if (defaultAgentSetting == null)
                    {
                        defaultAgentSetting = new Setting
                        {
                            Key = "DefaultAgentName",
                            Value = "Agent"
                        };
                        repository.Settings.Add(defaultAgentSetting);
                        repository.SaveChanges();
                    }
                    DefaultAgentName = !String.IsNullOrWhiteSpace(defaultAgentSetting.Value) ? defaultAgentSetting.Value : "Agent";
                }
                catch (Exception)
                {
                    DefaultAgentName = "Agent";
                }
            }
            return DefaultAgentName;
        }

        public static bool OverrideAgent()
        {
            if (OverrideAgentName == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "OverrideAgentName");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "OverrideAgentName",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    OverrideAgentName = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    OverrideAgentName = false;
                }
            }
            return OverrideAgentName.Value;
        }

        public static bool SaveIpAddress()
        {
            if (GatherIpAddress == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "GatherIPAddress");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "GatherIPAddress",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    GatherIpAddress = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    GatherIpAddress = false;
                }
            }
            return GatherIpAddress.Value;
        }

        public static bool ContinueChat()
        {
            if (AllowContinueChat == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "AllowContinueChat");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "AllowContinueChat",
                            Value = "true"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    AllowContinueChat = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    AllowContinueChat = true;
                }
            }
            return AllowContinueChat.Value;
        }

        public static int ContinueChatTimeout()
        {
            if (PausedChatTimeout == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "ContinueChatTimeout");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "ContinueChatTimeout",
                            Value = "30"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    PausedChatTimeout = Convert.ToInt32(setting.Value);
                }
                catch (Exception)
                {
                    PausedChatTimeout = 30;
                }
            }
            return PausedChatTimeout.Value;
        }

        public static bool Iframes()
        {
            if (AllowIframes == null)
            {
                try
                {
                    var repository = new Repository();
                    var setting = repository.Settings.FirstOrDefault(s => s.Key == "AllowIframes");
                    if (setting == null)
                    {
                        setting = new Setting
                        {
                            Key = "AllowIframes",
                            Value = "false"
                        };
                        repository.Settings.Add(setting);
                        repository.SaveChanges();
                    }
                    AllowIframes = setting.Value.ToLower() == "true";
                }
                catch (Exception)
                {
                    AllowIframes = false;
                }
            }
            return AllowIframes.Value;
        }

        public static string GetCustomErrorMessage()
        {
            if (String.IsNullOrEmpty(CustomErrorText))
            {
                try
                {
                    var repository = new Repository();
                    var customErrorText = repository.Settings.FirstOrDefault(s => s.Key == "CustomErrorText");
                    if (customErrorText == null)
                    {
                        customErrorText = new Setting
                        {
                            Key = "CustomErrorText",
                            Value = "Error connecting to Chat. Please try again later."
                        };
                        repository.Settings.Add(customErrorText);
                        repository.SaveChanges();
                    }
                    CustomErrorText = customErrorText.Value;
                }
                catch (Exception)
                {
                    CustomErrorText = "Error connecting to Chat. Please try again later.";
                }
            }
            return CustomErrorText;
        }

        public ActionResult RenderJavascript(string profile)
        {
            var fullDomain = "";

            if (Request.Url != null)
            {
                fullDomain = Request.Url.Host;
                LoggingService.GetInstance().LogNote(fullDomain);
            }

            LoggingService.GetInstance().LogNote("webchat.js...");
            if (!ChatServices.IsLicensed() || String.IsNullOrWhiteSpace(ChatServices.GetCicServerName))
            {
                LoggingService.GetInstance().LogNote("Returning null script");
                return null;
            }
            if (profile.ToLower() == "existingsession")
            {
                LoggingService.GetInstance().LogNote("Loading existing session...");
                var sessionId = Session.SessionID;
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.SessionId == sessionId && c.State != ChatState.Disconnected);
                if (webChat != null)
                {
                    profile = webChat.ProfileName;
                }
                else
                {
                    LoggingService.GetInstance().LogNote("Returning null script");
                    return null;
                }
            }
            try
            {
                var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());

                if (String.IsNullOrEmpty(fullDomain) || !ChatServices.AllowedWcbDomains.Any(a => a == fullDomain))
                {
                    fullDomain = WcbDomain;
                }

                //if (String.IsNullOrWhiteSpace(fullDomain) && Request.Url != null)
                //{
                //    fullDomain = Request.Url.Host;
                //}
                var secure = Request.IsSecureConnection;
                var formattedDomain = fullDomain.StartsWith("http://") || fullDomain.StartsWith("https://")
                    ? fullDomain
                    : secure ? "https://" + fullDomain : "http://" + fullDomain;
                //if (Request.Url != null && !Request.Url.IsDefaultPort)
                //{
                //    fullDomain += ":" + Request.Url.Port;
                //}
                var domainUri = new Uri(formattedDomain);
                var domainHost = domainUri.Host;
                var domain = domainHost;
                if (domainHost.Contains('.'))
                {
                    var domainParts = domainHost.Split('.');
                    var domainPartsCnt = domainParts.ToList().Count;
                    domain = domainPartsCnt == 2
                        ? domainHost
                        : domainParts[domainPartsCnt - 2] + "." + domainParts[domainPartsCnt - 1];
                }
                var isChatMinimized = false;
                if (chatProfile != null)
                {
                    LoggingService.GetInstance().LogNote("Chat profile: " + chatProfile.Name);
                    var widget = chatProfile.Widget;
                    if (widget != null)
                    {
                        LoggingService.GetInstance().LogNote("Chat widget: " + widget.Name);
                        if (!widget.IsActive)
                        {
                            LoggingService.GetInstance().LogNote("Widget is not active. Returning null script.");
                            return null;
                        }
                        var htmlModel = new DynamicChatHtmlViewModel
                        {
                            Domain = "//" + fullDomain,
                            PopOverlay = widget.PopOverlay,
                            Background = widget.Background,
                            TextColor = widget.TextColor,
                            Height = widget.Height,
                            Width = widget.Width,
                            UseIcon = widget.UseIcon,
                            IconPath = widget.IconPath,
                            ResumeIconPath = widget.ResumeIconPath,
                            UnavailableIconPath = widget.UnavailableIconPath,
                            IconWidth = widget.IconWidth,
                            LaunchText = widget.LaunchText,
                            ResumeLaunchText = widget.ResumeLaunchText,
                            UnavailableLaunchText = widget.UnavailableLaunchText,
                            PlaceHolderBackground = widget.PlaceHolderBackground,
                            OffsetX = widget.OffsetX,
                            OffsetY = widget.OffsetY,
                            Position = widget.Position,
                            ShowLoader = widget.ShowLoader,
                            Rounded = widget.Rounded,
                            Vertical = widget.Vertical,
                            TooltipColor = widget.TooltipColor,
                            TooltipText = widget.TooltipText,
                            ResumeTooltipText = widget.ResumeTooltipText,
                            UnavailableTooltipText = widget.UnavailableTooltipText,
                            ShowTooltip = widget.ShowTooltip
                        };
                        var html = RenderRazorViewToString(widget.IsSecondaryStyle ? "_DynamicHtml2" : "_DynamicHtml", htmlModel);
                        html = Regex.Replace(html, @"\s*(<[^>]+>)\s*", "$1", RegexOptions.Singleline);
                        var continueChatTimeRemaining = 0;
                        var chatState = ChatState.Trying;
                        if (ContinueChat())
                        {
                            //continueChatTimeRemaining = ContinueChatTimeout();
                            var sessionId = Session.SessionID;
                            var webChat = ChatServices.WebChats.FirstOrDefault(w => w.SessionId == sessionId && w.ProfileName == chatProfile.Name);
                            //if (webChat != null && (webChat.DateAnswered.HasValue))
                            if (webChat != null && (webChat.State == ChatState.Connected || webChat.State == ChatState.Paused || webChat.State == ChatState.Queued))
                            {
                                isChatMinimized = webChat.IsMinimized;
                                var now = DateTime.Now;
                                var remaining = ContinueChatTimeout();
                                if (webChat.DateEnded.HasValue)
                                {
                                    var elapsed = now.Subtract(webChat.DateEnded.Value);
                                    remaining = ContinueChatTimeout() - Convert.ToInt32(elapsed.TotalSeconds);
                                }

                                if (remaining > 0)
                                {
                                    continueChatTimeRemaining = remaining;
                                }
                                chatState = webChat.State;
                            }
                        }
                        var request = Request;
                        var ipAddress = SaveIpAddress() ? GetClientIpAddress(request) + ";" : "";
                        var fromUrl = request.UrlReferrer != null ? request.UrlReferrer.AbsoluteUri + ";" : "";
                        var userAgent = ShowUserAgentInUserData ? request.UserAgent + ";" : "";
                        var userData = fromUrl + userAgent + ipAddress;

                        var model = new DynamicChatScriptViewModel
                        {
                            Profile = profile,
                            DynamicHtml = html,
                            CheckForAgents = widget.CheckForAgents,
                            Domain = domain,
                            FullDomain = fullDomain,
                            RecycleTime = widget.RecycleTime,
                            StartTime = widget.StartTime,
                            Height = widget.Height,
                            Width = widget.Width,
                            MobileWidth = widget.MobileWidth,
                            MaxEstimatedWaitTime = widget.MaxEstimatedWaitTime,
                            ShowOnMobile = widget.ShowOnMobile,
                            ShowTooltipOnMobile = widget.ShowTooltipOnMobile,
                            UseIframe = widget.UseIframe,
                            RequiredAgentsAvailable = widget.RequiredAgentsAvailable,
                            ShowTooltip = widget.ShowTooltip,
                            ShowTooltipAtStart = widget.ShowTooltipAtStart,
                            Form = widget.Form,
                            UnavailableForm = widget.UnavailableForm,
                            PopOverlay = widget.PopOverlay,
                            LaunchInNewWindow = widget.LaunchInNewWindow,
                            ContinueChatTimeRemaining = continueChatTimeRemaining,
                            ContinueChat = ContinueChat(),
                            CustomCss = chatProfile.Template.CustomCss,
                            IncludeUserDataInAttributes = chatProfile.IncludeUserDataAsAttributes,
                            IncludeUserDataInCustomInfo = chatProfile.IncludeUserDataAsCustomInfo,
                            UserData = userData,
                            IsSecondaryStyle = widget.IsSecondaryStyle,
                            ChatState = chatState,
                            ShowUnavailableIfOpenNoAgents = widget.ShowUnavailableIfOpenNoAgents,
                            HasSchedules = chatProfile.Schedules.Any(i => i.IsActive && !i.MarkedForDeletion),
                            IsChatMinimized = isChatMinimized
                        };
                        if (EnableGoogleAnalytics && !String.IsNullOrWhiteSpace(GoogleAnalyticsTrackingId))
                        {
                            var sessionId = Session.SessionID;
                            var ip = GetClientIpAddress(request);
                            var ua = request.UserAgent;
                            GoogleAnalyticsServices.TrackEvent("Script Loaded", sessionId, ip, ua, profile);
                        }
                        LoggingService.GetInstance().LogNote("Script loaded successfully");
                        return PartialView("_DynamicScript", model);
                    }

                }
                LoggingService.GetInstance().LogNote("Chat Profile is null");
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }

            LoggingService.GetInstance().LogNote("Returning null script");
            return null;
            //var defaultModel = DefaultDynamicChatScript(profile, domain, fullDomain, DefaultDynamicHtml());
            //return PartialView("_DynamicScript", defaultModel);
        }

        public string DefaultDynamicHtml()
        {
            var htmlModel = new DynamicChatHtmlViewModel
            {
                PopOverlay = false,
                Background = "#333",
                TextColor = "#fff",
                Height = 480,
                Width = 640,
                UseIcon = true,
                IconPath = "/Content/Images/launch.png",
                IconWidth = 50,
                LaunchText = "",
                PlaceHolderBackground = "#333",
                OffsetX = 35,
                OffsetY = 75,
                Position = "bottom",
                ShowLoader = true
            };
            var html = RenderRazorViewToString("_DynamicHtml", htmlModel);
            html = Regex.Replace(html, @"\s*(<[^>]+>)\s*", "$1", RegexOptions.Singleline);
            return html;
        }

        public DynamicChatScriptViewModel DefaultDynamicChatScript(string profile, string domain, string fullDomain, string html)
        {
            var model = new DynamicChatScriptViewModel
            {
                Profile = profile,
                DynamicHtml = html,
                CheckForAgents = false,
                Domain = domain,
                FullDomain = fullDomain,
                RecycleTime = 0,
                StartTime = -1,
                Height = 480,
                Width = 640,
                MobileWidth = 767,
                MaxEstimatedWaitTime = 0,
                ShowOnMobile = true,
                UseIframe = false,
                ShowTooltip = false
            };
            return model;
        }

        // GET: Chat
        public ActionResult StandardChat(string profile = "", string user = "", string customInfo = "", string[] attributeNames = null, string[] attributeValues = null, bool embedded = true, bool testing = false, int testId = -1) // string sessionId = "",
        {
            try
            {
                var domain = "";

                if (Request.Url != null)
                {
                    domain = Request.Url.Host;
                    LoggingService.GetInstance().LogNote(domain);
                }

                var request = Request;
                var useIframe = !embedded; //!request.IsAjaxRequest();
                var userName = GetUserName(user);
                var defaultAgentName = GetAgentName();
                var overrideAgentName = OverrideAgent();
                var enableSendAndQueue = EnableSendAndQueue();
                var sessionId = Session.SessionID;
                var ipAddress = GetClientIpAddress(request);
                var fromUrl = request.UrlReferrer != null ? request.UrlReferrer.AbsoluteUri : "";
                var userAgent = request.UserAgent;
                var model = new ChatTemplateViewModel();
                var headerText = "";
                var headerLogo = "";
                var tooltipOverride = "";
                var launchTextOverride = "";
                var launchIconOverride = "";
                var template = new Template();

                if (testing)
                {
                    template = _repository.Templates.Find(testId);
                    headerText = template.HeaderText;
                    headerLogo = template.HeaderLogoPath;
                }
                else
                {
                    var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
                    if (chatProfile != null)
                    {
                        template = chatProfile.Template;
                        headerText = String.IsNullOrEmpty(chatProfile.HeaderText) ? template.HeaderText : chatProfile.HeaderText;
                        headerLogo = String.IsNullOrEmpty(chatProfile.HeaderLogoPath) ? template.HeaderLogoPath : chatProfile.HeaderLogoPath;
                        tooltipOverride = chatProfile.Widget.ResumeTooltipText;
                        launchIconOverride = chatProfile.Widget.ResumeIconPath;
                        launchTextOverride = chatProfile.Widget.ResumeLaunchText;
                        var userData = new UserData
                        {
                            FromUrl = fromUrl,
                            IpAddress = SaveIpAddress() ? ipAddress : "",
                            UserAgent = userAgent
                        };
                        var skipChatCreation = false;
                        if (ContinueChat())
                        {
                            var webChat = ChatServices.WebChats.FirstOrDefault(w => w.SessionId == sessionId && w.ProfileName == chatProfile.Name);
                            if (webChat != null)
                            {
                                userName = webChat.UserName;
                                if (webChat.State == ChatState.Paused || webChat.State == ChatState.Connected || webChat.State == ChatState.Queued)
                                {
                                    skipChatCreation = true;
                                }
                                else
                                {
                                    ChatServices.ProcessDisconnect(webChat.ConnectionId, false, true);
                                }
                                var others = ChatServices.WebChats.Where(w => w.SessionId == sessionId && w.ChatId != webChat.ChatId);
                                foreach (var other in others)
                                {
                                    ChatServices.ProcessDisconnect(other.ConnectionId, false, true);
                                }
                            }
                            else
                            {
                                var webChats = ChatServices.WebChats.Where(w => w.SessionId == sessionId).Select(c => c.ConnectionId).ToList();
                                foreach (var chat in webChats)
                                {
                                    ChatServices.ProcessDisconnect(chat, false, true);
                                }
                            }
                        }
                        else
                        {
                            var webChats = ChatServices.WebChats.Where(w => w.SessionId == sessionId).Select(c => c.ConnectionId).ToList();
                            foreach (var chat in webChats)
                            {
                                ChatServices.ProcessDisconnect(chat, false, true);
                            }
                        }
                        if (!skipChatCreation)
                        {
                            _repository.UsersData.Add(userData);
                            var chat = new Chat
                            {
                                DateCreated = DateTime.Now,
                                SessionId = sessionId,
                                UserData = userData,
                                Profile = chatProfile.Name
                            };
                            _repository.Chats.Add(chat);
                            _repository.SaveChanges();
                            //create request here
                            var chatAttributes = new Dictionary<string, string>();

                            try
                            {
                                if (attributeValues != null && attributeNames != null && attributeNames.ToList().Count == attributeValues.ToList().Count)
                                {
                                    for (int i = 0; i < attributeNames.ToList().Count; i++)
                                    {
                                        var key = attributeNames[i];
                                        var value = attributeValues[i];
                                        if (!chatAttributes.ContainsKey(key))
                                        {
                                            chatAttributes.Add(key, value);
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LoggingService.GetInstance().LogException(e);
                            }
                            chatAttributes.Add("WebChatId", chat.ChatId.ToString());
                            var skills = new Collection<RoutingContext>();
                            foreach (var skill in chatProfile.Skills)
                            {
                                skills.Add(new RoutingContext
                                {
                                    Category = "Product",
                                    Context = skill.DisplayName
                                });
                            }
                            var chatRequest = new ChatRequest
                            {
                                SupportedContentTypes = "text/plain",
                                Participant = new Participant
                                {
                                    Name = userName,
                                    Credentials = ""
                                },
                                TranscriptRequired = false,
                                EmailAddress = "unknown@unknown.com",
                                Target = chatProfile.Workgroup.DisplayName,
                                TargetType = "Workgroup",
                                Language = "en-us",
                                CustomInfo = customInfo,
                                Attributes = chatAttributes,
                                RoutingContexts = skills
                            };
                            var newWebChat = new WebChat
                            {
                                ChatId = chat.ChatId,
                                DateCreated = DateTime.Now,
                                SessionId = sessionId,
                                UserName = userName,
                                ChatRequest = chatRequest,
                                State = ChatState.Trying,
                                ProfileName = chatProfile.Name,
                                Messages = new Collection<ChatMessage>(),
                                PreviousConnectionIds = new Collection<string>(),
                                AllowAttachments = chatProfile.AllowAttachments
                            };
                            ChatServices.WebChats.Add(newWebChat);
                        }

                    }
                }

                if (String.IsNullOrEmpty(domain) || !ChatServices.AllowedWcbDomains.Any(a => a == domain))
                {
                    domain = WcbDomain;
                }

                if (template != null)
                {
                    model = new ChatTemplateViewModel
                    {
                        Profile = profile,
                        MessageArrows = template.MessageArrows,
                        CustomCss = template.CustomCss,
                        HeaderIcons = template.HeaderIcons,
                        HeaderLogoPath = headerLogo,
                        HeaderText = headerText,
                        IncludeHeader = template.IncludeHeader,
                        IncludePrint = template.IncludePrint,
                        IncludeTranscript = template.IncludeTranscript,
                        IncludeDisconnect = template.IncludeDisconnect,
                        PlaceholderText = template.PlaceholderText,
                        SendButtonIcon = template.SendButtonIcon,
                        SendIncludeIcon = template.SendIncludeIcon,
                        ShowInitials = template.ShowInitials,
                        ShowTime = template.ShowTime,
                        Title = String.IsNullOrEmpty(headerText) ? template.Title : headerText,
                        UserName = userName,
                        OverrideAgentName = overrideAgentName,
                        DefaultAgentName = defaultAgentName,
                        EnableSendAndQueueChatsBeforeAgent = enableSendAndQueue,
                        TestMode = testing,
                        ErrorText = GetCustomErrorMessage(),
                        Domain = "//" + domain,
                        SessionId = sessionId,
                        HideHeader = template.HideHeader,
                        HideSendButton = template.HideSendButton,
                        CloseButtonIconPath = template.CloseButtonIcon,
                        DisconnectButtonIconPath = template.DisconnectButtonIcon,
                        PrintButtonIconPath = template.PrintButtonIcon,
                        EmailButtonIconPath = template.EmailButtonIcon,
                        UseUnstyledHeaderIcons = template.UseUnstyledHeaderIcons,
                        LaunchTextOverride = launchTextOverride,
                        LaunchIconOverridePath = launchIconOverride,
                        TooltipOverrideText = tooltipOverride,
                        EnableAudioAlerts = EnableAudioAlerts,
                        EnableBrowserAlerts = EnableBrowserAlerts,
                        ShowOptionsButton = ShowOptionsButton,
                        CloseButtonTitle = CloseButtonTitle,
                        DisconnectButtonTitle = DisconnectButtonTitle,
                        ShowCustomInfoOnReload = ShowCustomInfoOnReload,
                        ShowCustomInfoOnLoad = ShowCustomInfoOnLoad
                        //ShowHardDisconnect = EnableHardDisconnect
                    };
                }
                else
                {
                    model = new ChatTemplateViewModel
                    {
                        MessageArrows = true,
                        CustomCss = "",
                        HeaderIcons = true,
                        HeaderLogoPath = "",
                        HeaderText = "Wcb Chat",
                        IncludePrint = false,
                        IncludeTranscript = false,
                        IncludeDisconnect = false,
                        PlaceholderText = "",
                        SendButtonIcon = "",
                        SendIncludeIcon = true,
                        ShowInitials = false,
                        Title = "Wcb Chat",
                        ShowTime = false,
                        UserName = userName,
                        TestMode = testing,
                        ErrorText = GetCustomErrorMessage(),
                        Domain = "//" + domain,
                        SessionId = sessionId,
                        CloseButtonTitle = CloseButtonTitle,
                        DisconnectButtonTitle = DisconnectButtonTitle
                    };
                }
                if (useIframe)
                {
                    return View("IframeChat", model);
                }
                return View(model);
            }
            catch (Exception e)
            {
                return Json(new {message = e.Message + ": " + e.StackTrace});
            }
        }

        public ActionResult ChatTranscript(string connectionId)
        {
            try
            {
                var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));

                if (chat == null)
                {
                    chat = ChatServices.DisconnectedWebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                }

                if (chat != null && Session.SessionID == chat.SessionId)
                {
                    List<ChatMessage> messages = chat.Messages.Any() ? chat.Messages.ToList() : new List<ChatMessage>();

                    var relatedChats = ChatServices.DisconnectedWebChats.Where(c => c.SessionId == chat.SessionId && c.ChatId != chat.ChatId).SelectMany(i => i.Messages).ToList();
                    if (relatedChats.Any())
                    {
                        messages.AddRange(relatedChats);
                    }
                    var distinctMessages = messages.GroupBy(x => x.Id).Select(y => y.First()).ToList();
                    if (distinctMessages.Any())
                    {
                        var body = "";

                        foreach (var chatMessage in distinctMessages.OrderBy(i => i.DateSent))
                        {
                            body += String.Format("<p>{0} {1} - {2}: {3}</p>", chatMessage.DateSent.ToShortDateString(), chatMessage.DateSent.ToLongTimeString(), chatMessage.Name, chatMessage.Text);
                        }
                        object model = body;
                        return View(model);
                    }
                }
            }
            catch (Exception)
            {

            }

            object error = "Error Loading Transcript";

            return View(error);
        }

        public JsonResult RegisterHubConnection(string connectionId)
        {
            var message = "";
            var success = false;
            var sessionId = Session.SessionID;
            var fromUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : "";
            var existingSession = false;
            var chatResumed = false;
            //if (Request.Headers.AllKeys.Contains("WCB-SESSION-ID"))
            //{
            //var sessionId = Request.Headers.Get("WCB-SESSION-ID");
            try
            {
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.SessionId == sessionId);
                if (webChat != null)
                {
                    if (!String.IsNullOrWhiteSpace(webChat.ConnectionId))
                    {
                        if (webChat.PreviousConnectionIds == null)
                        {
                            webChat.PreviousConnectionIds = new Collection<string>();
                        }
                        if (!webChat.PreviousConnectionIds.Contains(webChat.ConnectionId))
                        {
                            webChat.PreviousConnectionIds.Add(webChat.ConnectionId);
                        }
                    }
                    webChat.ConnectionId = connectionId;
                    var chat = _repository.Chats.Find(webChat.ChatId);
                    chat.ConnectionId = connectionId;
                    if (!String.IsNullOrEmpty(fromUrl) && chat.UserData.FromUrl != fromUrl)
                    {
                        chat.UserData.FromUrl = fromUrl;
                    }
                    if ((webChat.State == ChatState.Connected || webChat.State == ChatState.Paused || webChat.State == ChatState.Queued) &&
                        !String.IsNullOrEmpty(webChat.ConnectionId) && ContinueChat())
                    {
                        if (webChat.State == ChatState.Paused)
                        {
                            ChatServices.SendCustomSystemMessage(CustomMessageType.ResumedChat, webChat.UserName, connectionId, webChat);
                            chatResumed = true;
                        }
                        existingSession = true;
                        success = true;
                        webChat.State = webChat.State == ChatState.Queued ? ChatState.Queued : ChatState.Connected;

                        foreach (var chatMessage in webChat.Messages.OrderBy(m => m.Order).ToList())
                        {
                            if (chatMessage.Name.Equals("System", StringComparison.OrdinalIgnoreCase))
                            {
                                ChatServices.AddSystemMessage(chatMessage.Text, connectionId);
                            }
                            else if (chatMessage.Direction == "out")
                            {
                                ChatServices.AddVisitorMessage(chatMessage.Name, chatMessage.Text, connectionId, chatMessage.Id, chatMessage.DateSent.ToUniversalTime());
                            }
                            else
                            {
                                ChatServices.AddAgentMessage(chatMessage.Name, chatMessage.Text, chatMessage.ImgSrc, connectionId, chatMessage.Id, chatMessage.DateSent.ToUniversalTime());
                            }
                        }
                    }
                    else
                    {
                        var chatResponse = ChatServices.StartChat(webChat.ChatRequest);
                        if (chatResponse != null && chatResponse.Status.Type == "success")
                        {
                            webChat.ChatIdentifier = chatResponse.ChatID;
                            webChat.ParticipantId = chatResponse.ParticipantID;
                            webChat.State = ChatState.Queued;
                            webChat.DateCreated = DateTime.Now;
                            webChat.IsMinimized = false;
                            chat.ChatIdentifier = chatResponse.ChatID;
                            chat.ParticipantId = chatResponse.ParticipantID;
                            chat.DateCreated = DateTime.Now;
                            chat.QueueName = webChat.ChatRequest.Target;
                            _repository.SaveChanges();
                            success = true;
                            ChatServices.SendCustomSystemMessage(CustomMessageType.Connected, chat.QueueName, connectionId, webChat);
                            if (EnablePassHistoryToNewAgentOnRestart())
                            {
                                var dqChat = ChatServices.DisconnectedWebChats.Where(d => d.DateCreated.HasValue).OrderBy(d => d.DateCreated.Value).LastOrDefault(c => c.SessionId == sessionId);
                                if (dqChat != null)
                                {
                                    foreach (var chatMessage in dqChat.Messages.Where(c => !c.Name.Equals("System", StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.Order).ToList())
                                    {
                                        if (ChatServices.SendMessage(chatResponse.ParticipantID, chatMessage.Name + ": " + chatMessage.Text))
                                        {
                                            //
                                        }
                                    }
                                }
                            }
                            if (EnableReloadUserHistoryOnNewChat())
                            {
                                var dqChat = ChatServices.DisconnectedWebChats.Where(d => d.DateCreated.HasValue).OrderBy(d => d.DateCreated.Value).LastOrDefault(c => c.SessionId == sessionId);
                                if (dqChat != null)
                                {
                                    foreach (var chatMessage in dqChat.Messages.Where(c => !c.Name.Equals("System", StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.Order).ToList())
                                    {
                                        if (chatMessage.Direction == "out")
                                        {
                                            ChatServices.AddVisitorMessage(chatMessage.Name, chatMessage.Text, connectionId, chatMessage.Id, chatMessage.DateSent.ToUniversalTime());
                                        }
                                        else
                                        {
                                            ChatServices.AddAgentMessage(chatMessage.Name, chatMessage.Text, chatMessage.ImgSrc, connectionId, chatMessage.Id, chatMessage.DateSent.ToUniversalTime());
                                        }
                                    }
                                }
                            }
                            if (EnableReloadUnansweredChatHistory())
                            {
                                if (ChatServices.UnansweredChats.Any(u => u.SessionId == sessionId))
                                {
                                    var unanswered = ChatServices.UnansweredChats.FirstOrDefault(u => u.SessionId == sessionId);
                                    if (unanswered != null)
                                    {
                                        foreach (var chatMessage in unanswered.Messages.Where(i => !i.Name.Equals("System", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            if (!webChat.Messages.Any(m => m.Id == chatMessage.Id))
                                            {
                                                if (ChatServices.SendMessage(chatResponse.ParticipantID, chatMessage.Text))
                                                {
                                                    //
                                                }
                                                ChatServices.AddVisitorMessage(chatMessage.Name, chatMessage.Text, connectionId, chatMessage.Id, chatMessage.DateSent.ToUniversalTime());
                                                webChat.Messages.Add(chatMessage);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        webChat.ChatRequest = null;
                    }
                    if (existingSession)
                    {
                        ChatServices.AddAlert("Chat Resumed", "Existing chat resumed.", 1002);
                    }
                    else
                    {
                        ChatServices.AddAlert("Chat Created", "New chat created.", 1001);
                        if (EnableGoogleAnalytics && !String.IsNullOrWhiteSpace(GoogleAnalyticsTrackingId))
                        {
                            var request = Request;
                            var ipAddress = GetClientIpAddress(request);
                            var userAgent = request.UserAgent;
                            GoogleAnalyticsServices.TrackEvent("Chat Created", sessionId, ipAddress, userAgent, webChat.ProfileName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            //Here would be a good spot for an Alert
            return Json(new {existingSession, success, message, sessionId, chatResumed});
        }

        public ActionResult RestartDisconnectedSession(string connectionId, string profile = "", string user = "", string customInfo = "", string[] attributeNames = null, string[] attributeValues = null)
        {
            var message = "";
            var success = false;
            var sessionId = "";
            try
            {
                var request = Request;
                var userName = GetUserName(user);
                sessionId = Session.SessionID;
                var ipAddress = GetClientIpAddress(request);
                var fromUrl = request.UrlReferrer != null ? request.UrlReferrer.AbsoluteUri : "";
                var userAgent = request.UserAgent;

                var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
                if (chatProfile != null)
                {
                    var userData = new UserData
                    {
                        FromUrl = fromUrl,
                        IpAddress = SaveIpAddress() ? ipAddress : "",
                        UserAgent = userAgent
                    };
                    _repository.UsersData.Add(userData);
                    var chat = new Chat
                    {
                        DateCreated = DateTime.Now,
                        SessionId = sessionId,
                        UserData = userData,
                        Profile = chatProfile.Name
                    };
                    _repository.Chats.Add(chat);
                    _repository.SaveChanges();
                    //create request here
                    var chatAttributes = new Dictionary<string, string>();

                    try
                    {
                        if (attributeValues != null && (attributeNames != null && attributeNames.ToList().Count == attributeValues.ToList().Count))
                        {
                            for (int i = 0; i < attributeNames.ToList().Count; i++)
                            {
                                var key = attributeNames[i];
                                var value = attributeValues[i];
                                chatAttributes.Add(key, value);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    chatAttributes.Add("WebChatId", chat.ChatId.ToString());
                    var skills = new Collection<RoutingContext>();
                    foreach (var skill in chatProfile.Skills)
                    {
                        skills.Add(new RoutingContext
                        {
                            Category = "Product",
                            Context = skill.DisplayName
                        });
                    }
                    var chatRequest = new ChatRequest
                    {
                        SupportedContentTypes = "text/plain",
                        Participant = new Participant
                        {
                            Name = userName,
                            Credentials = ""
                        },
                        TranscriptRequired = false,
                        EmailAddress = "unknown@unknown.com",
                        Target = chatProfile.Workgroup.DisplayName,
                        TargetType = "Workgroup",
                        Language = "en-us",
                        CustomInfo = customInfo,
                        Attributes = chatAttributes,
                        RoutingContexts = skills
                    };
                    var webChat = new WebChat
                    {
                        ChatId = chat.ChatId,
                        DateCreated = DateTime.Now,
                        SessionId = sessionId,
                        UserName = userName,
                        ChatRequest = chatRequest,
                        State = ChatState.Trying,
                        ProfileName = chatProfile.Name,
                        Messages = new Collection<ChatMessage>(),
                        PreviousConnectionIds = new Collection<string>(),
                        AllowAttachments = chatProfile.AllowAttachments
                    };

                    var disconnectedChat =
                        ChatServices.DisconnectedWebChats.Where(d => d.SessionId == sessionId)
                            .OrderBy(d => d.DateCreated)
                            .LastOrDefault();
                    if (disconnectedChat != null)
                    {
                        if (disconnectedChat.PreviousConnectionIds != null &&
                            disconnectedChat.PreviousConnectionIds.Any(p => p != connectionId))
                        {
                            foreach (var previousConnectionId in disconnectedChat.PreviousConnectionIds.Where(p => p != connectionId))
                            {
                                webChat.PreviousConnectionIds.Add(previousConnectionId);
                            }
                        }
                        if (disconnectedChat.ConnectionId != connectionId)
                        {
                            webChat.PreviousConnectionIds.Add(disconnectedChat.ConnectionId);
                        }
                    }

                    ChatServices.WebChats.Add(webChat);

                    webChat.ConnectionId = connectionId;
                    chat.ConnectionId = connectionId;

                    var chatResponse = ChatServices.StartChat(webChat.ChatRequest);
                    if (chatResponse != null && chatResponse.Status.Type == "success")
                    {
                        webChat.ChatIdentifier = chatResponse.ChatID;
                        webChat.ParticipantId = chatResponse.ParticipantID;
                        webChat.State = ChatState.Queued;
                        webChat.DateCreated = DateTime.Now;
                        chat.ChatIdentifier = chatResponse.ChatID;
                        chat.ParticipantId = chatResponse.ParticipantID;
                        chat.DateCreated = DateTime.Now;
                        chat.QueueName = webChat.ChatRequest.Target;
                        _repository.SaveChanges();
                        success = true;
                        ChatServices.SendCustomSystemMessage(CustomMessageType.RestartedChat, chat.QueueName, connectionId, webChat);
                        if (webChat.PreviousConnectionIds != null && webChat.PreviousConnectionIds.Any())
                        {
                            foreach (var cid in webChat.PreviousConnectionIds.Where(p => p != connectionId))
                            {
                                ChatServices.SendCustomSystemMessage(CustomMessageType.RestartedChat, chat.QueueName, cid, webChat, true);
                            }
                        }
                        if (EnablePassHistoryToNewAgentOnRestart())
                        {
                            var dqChat = ChatServices.DisconnectedWebChats.Where(d => d.DateCreated.HasValue).OrderBy(d => d.DateCreated.Value).LastOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                            if (dqChat != null)
                            {
                                foreach (var chatMessage in dqChat.Messages.Where(c => !c.Name.Equals("System", StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.Order).ToList())
                                {
                                    if (ChatServices.SendMessage(chat.ParticipantId, chatMessage.Name + ": " + chatMessage.Text))
                                    {
                                        //
                                    }
                                }
                            }
                        }
                    }
                    webChat.ChatRequest = null;
                    ChatServices.AddAlert("Chat Restarted", "Chat Restarted, Re-entering queue.", 1001);
                    if (EnableGoogleAnalytics && !String.IsNullOrWhiteSpace(GoogleAnalyticsTrackingId))
                    {
                        GoogleAnalyticsServices.TrackEvent("Chat Restarted", sessionId, ipAddress, userAgent, profile);
                    }
                }

            }
            catch (Exception e)
            {
                message = e.Message;
            }
            //Here would be a good spot for an Alert
            return Json(new {success, message, sessionId});
        }

        //[OutputCache(Duration = 5, Location = OutputCacheLocation.Server, VaryByParam = "profile")]
        public JsonResult QueryAvailability(string profile) //, string sessionId)
        {
            try
            {
                var sessionId = Session.SessionID;
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.SessionId == sessionId);
                var hasSchedule = false;
                var message = "";
                var closed = true;
                if (webChat != null && webChat.DateAnswered.HasValue)
                {
                    return Json(new {success = true, agentsAvailable = 999, ewt = -1, closed = false, hasSchedule}, JsonRequestBehavior.AllowGet);
                }
                var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
                if (chatProfile != null)
                {
                    if (chatProfile.Schedules.Any(i => i.MarkedForDeletion))
                    {
                        try
                        {
                            foreach (var schedule in chatProfile.Schedules.Where(i => i.MarkedForDeletion).ToList())
                            {
                                var sched = chatProfile.Schedules.FirstOrDefault(i => i.Id == schedule.Id);
                                if (sched != null)
                                {
                                    chatProfile.Schedules.Remove(sched);                                    
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                    }
                    if (chatProfile.Schedules.Any(i => i.IsActive))
                    {
                        hasSchedule = true;
                        try
                        {
                            if (!ScheduleManager.ProfileAvailabilities.Any(i => i.ProfileId == chatProfile.ProfileId))
                            {
                                ScheduleManager.LoadScheduleForProfile(chatProfile);
                            }

                            var now = DateTime.UtcNow.AddHours(ScheduleManager.ScheduleDateTimeOffset);

                            var profileAvailability = ScheduleManager.ProfileAvailabilities.FirstOrDefault(i => i.ProfileId == chatProfile.ProfileId && i.ScheduleActiveRanges.Any(s => now >= s.StartDateTime && now <= s.EndDateTime));
                            if (profileAvailability != null)
                            {
                                var s = profileAvailability.ScheduleActiveRanges.Where(i => now >= i.StartDateTime && now <= i.EndDateTime).OrderBy(o => o.SchedulePriority).FirstOrDefault();
                                if (s != null)
                                {
                                    message = s.Message;
                                    if (!s.IsClosed)
                                    {
                                        closed = false;
                                    }
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                    }
                    var ewt = -1;
                    var agentsAvailable = 0;

                    if ((chatProfile.Widget.MaxEstimatedWaitTime > 0 || UseSimpleAvailabilityCheck) && (!hasSchedule || !closed))
                    {
                        var queueAvailability = ChatServices.QueryQueue(chatProfile.Workgroup.DisplayName);
                        if (chatProfile.Widget.MaxEstimatedWaitTime > 0)
                        {
                            ewt = queueAvailability.EstimatedWaitTime;
                        }
                        if (UseSimpleAvailabilityCheck)
                        {
                            agentsAvailable = queueAvailability.AgentsAvailable;
                        }
                    }
                    if (chatProfile.Widget.RequiredAgentsAvailable > 0 && !UseSimpleAvailabilityCheck && (!hasSchedule || !closed))
                    {
                        agentsAvailable = ChatServices.AgentsAvailable(chatProfile.ProfileId);
                    }
 
                    return Json(new { success = true, agentsAvailable, ewt, closed, hasSchedule, message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContinueSessionCheck(string sessionId)
        {
            try
            {
                sessionId = Session.SessionID;
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.SessionId == sessionId);
                if (webChat != null && (webChat.State == ChatState.Paused && webChat.DatePaused.HasValue))
                {
                    var now = DateTime.Now;
                    var elapsed = now.Subtract(webChat.DatePaused.Value);
                    var remaining = ContinueChatTimeout() - Convert.ToInt32(elapsed.TotalSeconds);
                    if (remaining > 0)
                    {
                        return Json(new { success = true, remainingTime = remaining }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveVisitorInfo(string profile, string[] fieldNames = null, string[] fieldValues = null, string customInfo = "", string[] attributeNames = null, string[] attributeValues = null)
        {
            try
            {
                LoggingService.GetInstance().LogNote(String.Format("SavingVisitorInfo for Profile: {0} with {1} Fields.", profile, fieldNames != null ? fieldNames.Length : 0));
                var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
                if (chatProfile != null)
                {
                    if (fieldValues != null && fieldNames != null && fieldNames.Length == fieldValues.Length)
                    {
                        var sb = new StringBuilder();
                        sb.Append(String.Format("Profile: {0}|Workgroup: {1}",chatProfile.Name, chatProfile.Workgroup.DisplayName));
                        for (var i = 0; i < fieldNames.Length; i++)
                        {
                            var key = fieldNames[i];
                            var value = fieldValues[i];
                            sb.Append(String.Format("|{0}: {1}", key, value));
                        }
                        var skills = chatProfile.Skills.Any() ? String.Join(",", chatProfile.Skills.Select(i => i.DisplayName)) : "";

                        var phoneNumberField = chatProfile.Widget.UnavailableForm.FormFields.FirstOrDefault(i => i.IsPhoneNumber);
                        var phoneNumber = "555-5555";
                        if (phoneNumberField != null)
                        {
                            var pnName = Array.IndexOf(fieldNames, phoneNumberField.Name);
                            if (pnName > -1)
                            {
                                phoneNumber = fieldValues[pnName];
                            }
                        }

                        var attrNames = "";
                        var attrValues = "";

                        if (attributeNames != null && attributeValues != null && attributeNames.Length == attributeValues.Length)
                        {
                            attrNames = String.Join("|", attributeNames);
                            attrValues = String.Join("|", attributeValues);
                        }

                        var visitorMessage = new VisitorMessage
                        {
                            DateCreated = DateTime.Now,
                            IsProcessed = false,
                            Workgroup = chatProfile.Workgroup.DisplayName,
                            Skills = skills,
                            Message = sb.ToString(),
                            PhoneNumber = phoneNumber,
                            CustomInfo = customInfo,
                            AttributeNames = attrNames,
                            AttributeValues = attrValues
                        };

                        _repository.VisitorMessages.Add(visitorMessage);
                        _repository.SaveChanges();

                        if (UseCallbackForVMs() && EnableGoogleAnalytics && !String.IsNullOrWhiteSpace(GoogleAnalyticsTrackingId))
                        {
                            var sessionId = Session.SessionID;
                            var request = Request;
                            var ipAddress = GetClientIpAddress(request);
                            var userAgent = request.UserAgent;
                            GoogleAnalyticsServices.TrackEvent("Callback Created", sessionId, ipAddress, userAgent, profile);
                        }
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new { success = false });
        }

        public ActionResult DownloadFile(string fileName)
        {
            try
            {
                //if (Request.Headers.AllKeys.Contains("WCB-SESSION-ID"))
                if (HttpContext.Session != null)
                {
                    //var sessionId = Request.Headers.Get("WCB-SESSION-ID");
                    var sessionId = HttpContext.Session.SessionID;
                    var path = Path.Combine(Server.MapPath("~/Files/" + sessionId), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        byte[] filedata = System.IO.File.ReadAllBytes(path);
                        string contentType = MimeMapping.GetMimeMapping(path);

                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = HttpUtility.UrlEncode(fileName),
                            Inline = true,
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(filedata, contentType);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateCallback(string profile, string message, string user = "", string phoneNumber = "555-5555", string customInfo = "", string[] attributeNames = null, string[] attributeValues = null)
        {
            var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
            if (chatProfile != null)
            {
                var skills = new Collection<CallbackRoutingContext>();
                foreach (var skill in chatProfile.Skills)
                {
                    skills.Add(new CallbackRoutingContext
                    {
                        Category = "Product",
                        Context = skill.DisplayName
                    });
                }

                var attributes = new Dictionary<string, string>();

                try
                {
                    if (attributeValues != null && attributeNames != null && attributeNames.ToList().Count == attributeValues.ToList().Count)
                    {
                        for (int i = 0; i < attributeNames.ToList().Count; i++)
                        {
                            var key = attributeNames[i];
                            var value = attributeValues[i];
                            attributes.Add(key, value);
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingService.GetInstance().LogException(e);
                }

                var subject = message.Length > 2000 ? message.Substring(0, 1999) : message;

                var callbackRequest = new CallbackRequest()
                {
                    Participant = new CallbackParticipant
                    {
                        Name = GetUserName(user),
                        Credentials = "",
                        Telephone = phoneNumber
                    },
                    Target = chatProfile.Workgroup.DisplayName,
                    TargetType = "Workgroup",
                    Language = "en-us",
                    CustomInfo = customInfo,
                    Attributes = attributes,
                    Subject = subject,
                    RoutingContexts = skills
                };

                var callbackResponse = ChatServices.CreateCallback(callbackRequest);
                if (callbackResponse != null && callbackResponse.Status.Type == "success")
                {
                    if (EnableGoogleAnalytics && !String.IsNullOrWhiteSpace(GoogleAnalyticsTrackingId))
                    {
                        var sessionId = Session.SessionID;
                        var request = Request;
                        var ipAddress = GetClientIpAddress(request);
                        var userAgent = request.UserAgent;
                        GoogleAnalyticsServices.TrackEvent("Callback Created", sessionId, ipAddress, userAgent, profile);
                    }

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult RequestTranscriptEmail(string connectionId, string email)
        {
            var success = false;
            try
            {
                var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));

                if (chat == null)
                {
                    chat = ChatServices.DisconnectedWebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                }

                if (chat != null && Session.SessionID == chat.SessionId)
                {
                    List<ChatMessage> messages = chat.Messages.Any() ? chat.Messages.ToList() : new List<ChatMessage>();

                    var relatedChats = ChatServices.DisconnectedWebChats.Where(c => c.SessionId == chat.SessionId && c.ChatId != chat.ChatId).SelectMany(i => i.Messages).ToList();
                    if (relatedChats.Any())
                    {
                        messages.AddRange(relatedChats);
                    }
                    var distinctMessages = messages.GroupBy(x => x.Id).Select(y => y.First()).ToList();
                    if (distinctMessages.Any())
                    {
                        var lineBreak = "\r\n";
                        var body = "Chat Transcript: " + lineBreak;
                        body += lineBreak;
                        //body += chat.DateCreated.HasValue ? chat.DateCreated.Value.ToShortDateString() : DateTime.Now.ToShortDateString();
                        body += lineBreak;
                        var subject = EmailTranscriptSubject;

                        foreach (var chatMessage in distinctMessages.OrderBy(i => i.DateSent))
                        {
                            body += String.Format("{0} {1} - {2}: {3}{4}{4}", chatMessage.DateSent.ToShortDateString(), chatMessage.DateSent.ToLongTimeString(), chatMessage.Name, chatMessage.Text, lineBreak);
                        }

                        Task.Run(() => EmailServices.SendTranscript(email, subject, body));
                        success = true;
                    }
                }
            }
            catch (Exception)
            {

            }
            return Json(new {success});
        }

        public static string GetClientIpAddress(HttpRequestBase request)
        {
            try
            {
                var userHostAddress = request.UserHostAddress;
                // Attempt to parse.  If it fails, we catch below and return "0.0.0.0"
                // Could use TryParse instead, but I wanted to catch all exceptions
                IPAddress.Parse(userHostAddress);
                var xForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(xForwardedFor))
                    return userHostAddress;

                // Get a list of public ip addresses in the X_FORWARDED_FOR variable
                var publicForwardingIps = xForwardedFor.Split(',').Where(ip => !IsPrivateIpAddress(ip)).ToList();

                // If we found any, return the last one, otherwise return the user host address
                return publicForwardingIps.Any() ? publicForwardingIps.Last() : userHostAddress;
            }
            catch (Exception)
            {
                // Always return all zeroes for any failure (my calling code expects it)
                return "0.0.0.0";
            }
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        //Added for Embedded Chat
        public JsonResult NewChat(string profile, string user = "", string customInfo = "", string[] attributeNames = null, string[] attributeValues = null)
        {
            try
            {
                var request = Request;
                var userName = GetUserName(user);
                var defaultAgentName = GetAgentName();
                var overrideAgentName = OverrideAgent();
                var enableSendAndQueue = EnableSendAndQueue();
                var sessionId = Session.SessionID;
                var ipAddress = GetClientIpAddress(request);
                var fromUrl = request.UrlReferrer != null ? request.UrlReferrer.AbsoluteUri : "";
                var userAgent = request.UserAgent;

                var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
                if (chatProfile != null)
                {
                    var userData = new UserData
                    {
                        FromUrl = fromUrl,
                        IpAddress = SaveIpAddress() ? ipAddress : "",
                        UserAgent = userAgent
                    };
                    var skipChatCreation = false;
                    if (ContinueChat())
                    {
                        var webChat = ChatServices.WebChats.FirstOrDefault(w => w.SessionId == sessionId && w.ProfileName == chatProfile.Name);
                        if (webChat != null)
                        {
                            userName = webChat.UserName;
                            if (webChat.State == ChatState.Paused || webChat.State == ChatState.Connected || webChat.State == ChatState.Queued)
                            {
                                skipChatCreation = true;
                            }
                            else
                            {
                                ChatServices.ProcessDisconnect(webChat.ConnectionId, false, true);
                            }
                            var others = ChatServices.WebChats.Where(w => w.SessionId == sessionId && w.ChatId != webChat.ChatId);
                            foreach (var other in others)
                            {
                                ChatServices.ProcessDisconnect(other.ConnectionId, false, true);
                            }
                        }
                        else
                        {
                            var webChats = ChatServices.WebChats.Where(w => w.SessionId == sessionId).Select(c => c.ConnectionId).ToList();
                            foreach (var chat in webChats)
                            {
                                ChatServices.ProcessDisconnect(chat, false, true);
                            }
                        }
                    }
                    else
                    {
                        var webChats = ChatServices.WebChats.Where(w => w.SessionId == sessionId).Select(c => c.ConnectionId).ToList();
                        foreach (var chat in webChats)
                        {
                            ChatServices.ProcessDisconnect(chat, false, true);
                        }
                    }
                    if (!skipChatCreation)
                    {
                        _repository.UsersData.Add(userData);
                        var chat = new Chat
                        {
                            DateCreated = DateTime.Now,
                            SessionId = sessionId,
                            UserData = userData,
                            Profile = chatProfile.Name
                        };
                        _repository.Chats.Add(chat);
                        _repository.SaveChanges();
                        //create request here
                        var chatAttributes = new Dictionary<string, string>();

                        try
                        {
                            if (attributeValues != null && attributeNames != null && attributeNames.ToList().Count == attributeValues.ToList().Count)
                            {
                                for (int i = 0; i < attributeNames.ToList().Count; i++)
                                {
                                    var key = attributeNames[i];
                                    var value = attributeValues[i];
                                    if (!chatAttributes.ContainsKey(key))
                                    {
                                        chatAttributes.Add(key, value);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                        chatAttributes.Add("WebChatId", chat.ChatId.ToString());
                        var skills = new Collection<RoutingContext>();
                        foreach (var skill in chatProfile.Skills)
                        {
                            skills.Add(new RoutingContext
                            {
                                Category = "Product",
                                Context = skill.DisplayName
                            });
                        }
                        var chatRequest = new ChatRequest
                        {
                            SupportedContentTypes = "text/plain",
                            Participant = new Participant
                            {
                                Name = userName,
                                Credentials = ""
                            },
                            TranscriptRequired = false,
                            EmailAddress = "unknown@unknown.com",
                            Target = chatProfile.Workgroup.DisplayName,
                            TargetType = "Workgroup",
                            Language = "en-us",
                            CustomInfo = customInfo,
                            Attributes = chatAttributes,
                            RoutingContexts = skills
                        };
                        var newWebChat = new WebChat
                        {
                            ChatId = chat.ChatId,
                            DateCreated = DateTime.Now,
                            SessionId = sessionId,
                            UserName = userName,
                            ChatRequest = chatRequest,
                            State = ChatState.Trying,
                            ProfileName = chatProfile.Name,
                            Messages = new Collection<ChatMessage>(),
                            PreviousConnectionIds = new Collection<string>(),
                            AllowAttachments = chatProfile.AllowAttachments
                        };
                        ChatServices.WebChats.Add(newWebChat);
                    }
                }

                return Json(new { success = true, message = "" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message + ": " + e.StackTrace });
            }
        }

        public JsonResult AvailabilityCheck(string profile)
        {
            var success = false;
            var chatAvailable = false;
            var existingChat = false;

            try
            {
                var sessionId = Session.SessionID;
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.SessionId == sessionId);
                var hasSchedule = false;
                var closed = true;

                if (webChat != null && webChat.DateAnswered.HasValue)
                {
                    existingChat = true;
                    //return Json(new { success = true, chatAvailable = true, existingChat = true }, JsonRequestBehavior.AllowGet);
                }
                var chatProfile = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profile.ToLower());
                if (chatProfile != null)
                {
                    if (chatProfile.Schedules.Any(i => i.MarkedForDeletion))
                    {
                        try
                        {
                            foreach (var schedule in chatProfile.Schedules.Where(i => i.MarkedForDeletion).ToList())
                            {
                                var sched = chatProfile.Schedules.FirstOrDefault(i => i.Id == schedule.Id);
                                if (sched != null)
                                {
                                    chatProfile.Schedules.Remove(sched);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                    }
                    if (chatProfile.Schedules.Any(i => i.IsActive))
                    {
                        hasSchedule = true;
                        try
                        {
                            if (!ScheduleManager.ProfileAvailabilities.Any(i => i.ProfileId == chatProfile.ProfileId))
                            {
                                ScheduleManager.LoadScheduleForProfile(chatProfile);
                            }

                            var now = DateTime.UtcNow.AddHours(ScheduleManager.ScheduleDateTimeOffset);

                            var profileAvailability = ScheduleManager.ProfileAvailabilities.FirstOrDefault(i => i.ProfileId == chatProfile.ProfileId && i.ScheduleActiveRanges.Any(s => now >= s.StartDateTime && now <= s.EndDateTime));
                            if (profileAvailability != null)
                            {
                                var s = profileAvailability.ScheduleActiveRanges.Where(i => now >= i.StartDateTime && now <= i.EndDateTime).OrderBy(o => o.SchedulePriority).FirstOrDefault();
                                if (s != null)
                                {
                                    if (!s.IsClosed)
                                    {
                                        closed = false;
                                    }
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                    }
                    var ewt = -1;
                    var agentsAvailable = 0;

                    if ((chatProfile.Widget.MaxEstimatedWaitTime > 0 || UseSimpleAvailabilityCheck) && (!hasSchedule || !closed))
                    {
                        var queueAvailability = ChatServices.QueryQueue(chatProfile.Workgroup.DisplayName);
                        if (chatProfile.Widget.MaxEstimatedWaitTime > 0)
                        {
                            ewt = queueAvailability.EstimatedWaitTime;
                        }
                        if (UseSimpleAvailabilityCheck)
                        {
                            agentsAvailable = queueAvailability.AgentsAvailable;
                        }
                    }
                    if (chatProfile.Widget.RequiredAgentsAvailable > 0 && !UseSimpleAvailabilityCheck && (!hasSchedule || !closed))
                    {
                        agentsAvailable = ChatServices.AgentsAvailable(chatProfile.ProfileId);
                    }

                    var isOpen = !hasSchedule || !closed;
                    var isAvailable = !chatProfile.Widget.CheckForAgents || agentsAvailable >= chatProfile.Widget.RequiredAgentsAvailable && ewt <= chatProfile.Widget.MaxEstimatedWaitTime;

                    chatAvailable = isOpen && isAvailable;
                    success = true;
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }

            return Json(new { success, chatAvailable, existingChat, showCustomInfoOnLoad = ShowCustomInfoOnLoad, showCustomInfoOnReload = ShowCustomInfoOnReload }, JsonRequestBehavior.AllowGet);
        }

    }
}