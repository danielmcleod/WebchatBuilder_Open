using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using WebchatBuilder.Controllers;
using WebchatBuilder.DataModels;
using WebchatBuilder.Hubs;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;
using WebChatBuilderModels.Shared;

namespace WebchatBuilder.Services
{
    public static class ChatServices
    {
        private static string[] _allowedIpAddresses;

        public static string[] AllowedIpAddresses
        {
            get
            {
                if (_allowedIpAddresses != null && _allowedIpAddresses.Any())
                {
                    return _allowedIpAddresses;
                }
                try
                {
                    _allowedIpAddresses = Convert.ToString(ConfigurationManager.AppSettings["AllowedIPAddresses"]).Split(',');
                    return _allowedIpAddresses;
                }
                catch (Exception)
                {
                    
                }
                return new string[0];
            }
        }

        private static string[] _blockedIpAddresses;

        public static string[] BlockedIpAddresses
        {
            get
            {
                if (_blockedIpAddresses != null && _blockedIpAddresses.Any())
                {
                    return _blockedIpAddresses;
                }
                try
                {
                    _blockedIpAddresses = Convert.ToString(ConfigurationManager.AppSettings["BlockedIpAddresses"]).Split(',');
                    return _blockedIpAddresses;
                }
                catch (Exception)
                {

                }
                return new string[0];
            }
        }

        private static string[] _allowedDomains;

        public static string[] AllowedDomains
        {
            get
            {
                if (_allowedDomains != null && _allowedDomains.Any())
                {
                    return _allowedDomains;
                }
                try
                {
                    _allowedDomains = Convert.ToString(ConfigurationManager.AppSettings["AllowedDomains"]).Split(',');
                    return _allowedDomains;
                }
                catch (Exception)
                {

                }
                return new string[0];
            }
        }

        private static string[] _allowedWcbDomains;

        public static string[] AllowedWcbDomains
        {
            get
            {
                if (_allowedWcbDomains != null && _allowedWcbDomains.Any())
                {
                    return _allowedWcbDomains;
                }
                try
                {
                    _allowedWcbDomains = Convert.ToString(ConfigurationManager.AppSettings["AllowedWcbDomains"]).Split(',');
                    return _allowedWcbDomains;
                }
                catch (Exception)
                {

                }
                return new string[0];
            }
        }
        
        public static Collection<WebChat> WebChats = new Collection<WebChat>();
        public static Collection<WebChat> DisconnectedWebChats = new Collection<WebChat>();
        public static Collection<ChatMessage> QueuedMessages = new Collection<ChatMessage>();
        public static Collection<UnansweredChat> UnansweredChats = new Collection<UnansweredChat>(); 
        public static Collection<VisitorMessageChat> VisitorMessageChats = new Collection<VisitorMessageChat>();

        private static string _cicServer;
        private static string _currentCicServer;

        public static bool Reconnecting { get; set; }

        public static bool GettingConfig { get; set; }

        public static string GetCicServerName
        {
            get { return _currentCicServer; }
        }

        public static bool Licensed { get; set; }

        public static string ConfigBaseUrl { get; set; }

        public static string BaseUrl
        {
            //http port 8114 default https port 3508 default
            get
            {
                var port = ConfigurationManager.AppSettings["CicServerPort"];
                var protocol = ConfigurationManager.AppSettings["CicServerProtocol"];
                if (String.IsNullOrEmpty(_currentCicServer))
                {
                    CicServer();
                    _currentCicServer = _cicServer;
                }
                else
                {
                    if (_currentCicServer != _cicServer && !String.IsNullOrEmpty(_cicServer) && !Reconnecting)
                    {
                        Reconnecting = true;
                        SwitchoverDetected();
                        var reconnected = false;
                        while (!reconnected)
                        {
                            var urlBase = String.Format("{0}://{1}:{2}", protocol, _cicServer, port);
                            if (!GettingConfig && GetConfig(urlBase))
                            {
                                foreach (var webChat in WebChats)
                                {
                                    if (ReconnectChat(webChat.ChatIdentifier))
                                    {
                                        ReconnectToCic(webChat.ConnectionId);
                                    }
                                }
                                reconnected = true;
                                _currentCicServer = _cicServer;
                                Reconnecting = false;
                            }
                        }
                    }
                }
                var ipOrName = _currentCicServer;
                var baseUrl = String.Format("{0}://{1}:{2}", protocol, ipOrName, port);
                if (!String.IsNullOrWhiteSpace(ipOrName))
                {
                    ConfigBaseUrl = baseUrl;
                }
                return baseUrl;
            }
        }

        public static string LocalServiceUrl
        {
            get
            {
                var port = ConfigurationManager.AppSettings["WcbServicePort"];
                if (String.IsNullOrWhiteSpace(port))
                {
                    port = "8088";
                }
                return "http://localhost:" + port + "/services/";
            }
        }
        //private const string BaseUrl = "http://172.16.1.47:8114";
        //private const string BaseUrl = "http://192.168.1.160:8114";
        //private const string BaseUrl = "https://192.168.1.160:3508"; Need Cert and use host name, or trust all certs

        public static IHubContext HubContext = GlobalHost.ConnectionManager.GetHubContext<WebChatHub>();
        public static IHubContext AlertHubContext = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();

        public static void AddAlert(string title, string description, int alertId)
        {
            AlertHubContext.Clients.All.addAlert(title, description, alertId);
        }

        public static string GetInitialsFromName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    if (name.Contains(" "))
                    {
                        var nameSplit = name.Split(' ');
                        if (nameSplit.Length > 1)
                        {
                            return nameSplit[0].Substring(0, 1) + nameSplit[1].Substring(0, 1);
                        }
                        return name.Substring(0, 1);
                    }
                    if (name.Contains("."))
                    {
                        var nameSplit = name.Split('.');
                        if (nameSplit.Length > 1)
                        {
                            return nameSplit[0].Substring(0, 1) + nameSplit[1].Substring(0, 1);
                        }
                        return name.Substring(0, 1);
                    }
                }
                catch (Exception)
                {
                }

                return name.Substring(0, 1);
            }

            return "";
        }

        public static void SendCustomSystemMessage(CustomMessageType systemMessageType, string param, string connectionId, WebChat chat = null, bool isDuplicate = false)
        {
            if (ChatController.EnableCustomSystemMessages())
            {
                var message = "";
                var repository = new Repository();
                switch (systemMessageType)
                {
                        case CustomMessageType.Connected:
                            var connected = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.Connected);
                            if (connected != null && connected.IsEnabled)
                            {
                                message = connected.Message.Replace("%PARAM%",param);
                                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
                            }
                            break;
                        case CustomMessageType.AgentJoined:
                            var agentJoined = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.AgentJoined);
                            if (agentJoined != null && agentJoined.IsEnabled)
                            {
                                message = agentJoined.Message.Replace("%PARAM%", param);
                                if (agentJoined.Message.Contains("%IMAGE%"))
                                {
                                    var imgsrc = "/Content/Images/agent.png";
                                    if (File.Exists(HostingEnvironment.MapPath("~/Images/Agents/" + param + ".png")))
                                    {
                                        imgsrc = "/Images/Agents/" + param + ".png";
                                    }
                                    var img = String.Format("{0}{1}{2}{3}", "<div class=\"chat-msg-icon\"><img src=\"", "//" + ChatController.WcbDomain.Trim('/'), imgsrc, "\" /></div>");
                                    var msg = message.Replace("%IMAGE%", "");
                                    message = String.Format("{0}{1}{2}{3}", "<span class=\"wcb-pre-agent-image\">",msg,"</span>", img);
                                }

                                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
                            }
                            break;
                        case CustomMessageType.AgentDisconnect:
                            var agentDisconnected = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.AgentDisconnect);
                            if (agentDisconnected != null && agentDisconnected.IsEnabled)
                            {
                                message = agentDisconnected.Message.Replace("%PARAM%", param);
                                if (agentDisconnected.Message.Contains("%IMAGE%"))
                                {
                                    var imgsrc = "/Content/Images/agent.png";
                                    if (File.Exists(HostingEnvironment.MapPath("~/Images/Agents/" + param + ".png")))
                                    {
                                        imgsrc = "/Images/Agents/" + param + ".png";
                                    }
                                    var img = String.Format("{0}{1}{2}{3}", "<div class=\"chat-msg-icon\"><img src=\"", "//" + ChatController.WcbDomain.Trim('/'), imgsrc, "\" /></div>");
                                    var msg = message.Replace("%IMAGE%", "");
                                    message = String.Format("{0}{1}{2}{3}", "<span class=\"wcb-pre-agent-image\">", msg, "</span>", img);
                                }

                                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
                            }
                            break;
                        case CustomMessageType.InactiveDisconnect:
                            var inactive = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.InactiveDisconnect);
                            if (inactive != null && inactive.IsEnabled)
                            {
                                message = inactive.Message.Replace("%PARAM%", param);
                                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
                            }
                            break;
                        case CustomMessageType.RestartedChat:
                            var restarted = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.RestartedChat);
                            if (restarted != null && restarted.IsEnabled)
                            {
                                message = restarted.Message.Replace("%PARAM%", param);
                                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
                            }
                            break;
                        case CustomMessageType.PausedChat:
                            var paused = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.PausedChat);
                            if (paused != null && paused.IsEnabled)
                            {
                                //var chat = WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                                if (chat != null)
                                {
                                    message = paused.Message.Replace("%PARAM%", param);
                                    SendMessage(chat.ParticipantId, message);
                                }
                            }
                            break;
                        case CustomMessageType.ResumedChat:
                            var resumed = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.ResumedChat);
                            if (resumed != null && resumed.IsEnabled)
                            {
                                //var chat = WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                                if (chat != null)
                                {
                                    message = resumed.Message.Replace("%PARAM%", param);
                                    SendMessage(chat.ParticipantId, message);
                                }
                            }
                            break;
                        case CustomMessageType.VisitorDisconnect:
                            var visitorDisconnect = repository.CustomMessages.FirstOrDefault(c => c.Type == CustomMessageType.VisitorDisconnect);
                            if (visitorDisconnect != null && visitorDisconnect.IsEnabled)
                            {
                                message = visitorDisconnect.Message;//.Replace("%PARAM%", param);
                                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
                            }
                            break;
                }

                if (!isDuplicate && chat != null && !String.IsNullOrWhiteSpace(message) && systemMessageType != CustomMessageType.ResumedChat && systemMessageType != CustomMessageType.PausedChat)
                {
                    var initials = "";
                    var messageId = GetNewMessageId();
                    var now = DateTime.Now;
                    chat.Messages.Add(new ChatMessage
                    {
                        DateSent = now,
                        Direction = "in",
                        ImgSrc = "",
                        Initials = initials,
                        Name = "System",
                        Order = !chat.Messages.Any() ? 1 : chat.Messages.Count + 1,
                        Sent = true,
                        Text = message,
                        Id = messageId
                    });
                }
            }
        }

        public static void SendMessage(WebChat chat, string connectionId, string name, string message)
        {
            if (chat.State == ChatState.Paused)
            {
                chat.State = ChatState.Connected;
            }
            if (SendMessage(chat.ParticipantId, message))
            {
                var initials = GetInitialsFromName(name);
                const string imgsrc = "/Content/Images/user.png";
                if (chat.Messages == null)
                {
                    chat.Messages = new Collection<ChatMessage>();
                }
                var messageId = GetNewMessageId(chat.ChatId);
                var now = DateTime.Now;
                chat.Messages.Add(new ChatMessage
                {
                    DateSent = now,
                    Direction = "out",
                    ImgSrc = imgsrc,
                    Initials = initials,
                    Name = name,
                    Order = !chat.Messages.Any() ? 1 : chat.Messages.Count + 1,
                    Sent = true,
                    Text = message,
                    Id = messageId
                });
                HubContext.Clients.Client(connectionId).addNewMessageToPage(name, message, imgsrc, "out", initials, messageId, now.ToUniversalTime());
                LoggingService.GetInstance().LogMessage(name, connectionId, chat.SessionId, "WebChatHub::connectionId::" + message, "out");
                if (connectionId != chat.ConnectionId)
                {
                    HubContext.Clients.Client(chat.ConnectionId).addNewMessageToPage(name, message, imgsrc, "out", initials, messageId, now.ToUniversalTime());
                    LoggingService.GetInstance().LogMessage(name, chat.ConnectionId, chat.SessionId, "WebChatHub::chat.connectionId::" + message, "out");
                }

                if (chat.PreviousConnectionIds != null && chat.PreviousConnectionIds.Any())
                {
                    foreach (var clientConnectionId in chat.PreviousConnectionIds.Where(p => p != connectionId && p != chat.ConnectionId))
                    {
                        HubContext.Clients.Client(clientConnectionId).addNewMessageToPage(name, message, imgsrc, "out", initials, messageId, now.ToUniversalTime());
                        LoggingService.GetInstance().LogMessage(name, clientConnectionId, chat.SessionId, "WebChatHub::clientConnectionId::" + message, "out");
                    }
                }
            }
        }

        public static void QueueMessageToAgent(string connectionId, string message, string name)
        {
            var initials = GetInitialsFromName(name);
            const string imgsrc = "/Content/Images/user.png";
            var relatedMessages = QueuedMessages.Where(m => m.ConnectionId == connectionId).ToList();
            var chatMessage = new ChatMessage
            {
                DateSent = DateTime.Now,
                Direction = "out",
                ImgSrc = imgsrc,
                Initials = initials,
                Name = name,
                Order = !relatedMessages.Any() ? 1 : relatedMessages.Count + 1,
                Sent = false,
                Text = message,
                Id = Guid.NewGuid().ToString()
            };
            QueuedMessages.Add(chatMessage);
        }

        public static void Receive(string name, string message, string imgsrc, string connectionId, bool isSystemMsg = false)
        {
            var initials = "";
            var messageId = GetNewMessageId();
            var now = DateTime.Now;
            if (isSystemMsg)
            {
                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
            }
            else
            {
                initials = GetInitialsFromName(name);
                if (imgsrc == "")
                {
                    imgsrc = "/Content/Images/agent.png";
                }
                HubContext.Clients.Client(connectionId).addNewMessageToPage(name, message, imgsrc, "in", initials, messageId, now.ToUniversalTime());
                LoggingService.GetInstance().LogMessage(name, connectionId, "N/A", message, "in");
            }
            try
            {
                var chat = WebChats.FirstOrDefault(c => c.ConnectionId == connectionId);
                if (chat != null)
                {
                    if (chat.AgentName != name)
                    {
                        chat.AgentName = name;
                    }
                    if (chat.State != ChatState.Connected && !isSystemMsg)
                    {
                        chat.State = ChatState.Connected;
                        chat.DateAnswered = now;
                    }
                    if (chat.Messages == null)
                    {
                        chat.Messages = new Collection<ChatMessage>();
                    }
                    chat.Messages.Add(new ChatMessage
                    {
                        DateSent = now,
                        Direction = "in",
                        ImgSrc = imgsrc,
                        Initials = initials,
                        Name = isSystemMsg ? "System" : name,
                        Order = !chat.Messages.Any() ? 1 : chat.Messages.Count + 1,
                        Sent = true,
                        Text = message,
                        Id = messageId
                    });

                    if (chat.PreviousConnectionIds != null && chat.PreviousConnectionIds.Any())
                    {
                        if (isSystemMsg)
                        {
                            foreach (var clientConnectionId in chat.PreviousConnectionIds.Where(p => p != connectionId))
                            {
                                HubContext.Clients.Client(clientConnectionId).addNewSystemMessageToPage(message);
                            }
                        }
                        else
                        {
                            foreach (var clientConnectionId in chat.PreviousConnectionIds.Where(p => p != connectionId))
                            {
                                HubContext.Clients.Client(clientConnectionId).addNewMessageToPage(name, message, imgsrc, "in", initials, messageId, now.ToUniversalTime());
                                LoggingService.GetInstance().LogMessage(name, clientConnectionId, chat.SessionId, message, "in");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static void AddAgentMessage(string name, string message, string imgsrc, string connectionId, string messageId, DateTime sent)
        {
            if (String.IsNullOrWhiteSpace(imgsrc))
            {
                imgsrc = "/Content/Images/agent.png";
            }
            var initials = GetInitialsFromName(name);
            HubContext.Clients.Client(connectionId).addNewMessageToPage(name, message, imgsrc, "in", initials, messageId, sent);
        }

        public static void AddVisitorMessage(string name, string message, string connectionId, string messageId, DateTime sent)
        {
            var initials = GetInitialsFromName(name);
            const string imgsrc = "/Content/Images/user.png";
            HubContext.Clients.Client(connectionId).addNewMessageToPage(name, message, imgsrc, "out", initials, messageId, sent);
        }

        public static void AddSystemMessage(string message, string connectionId)
        {
            HubContext.Clients.Client(connectionId).addNewSystemMessageToPage(message);
        }

        public static void EnableSendButton(string connectionId)
        {
            HubContext.Clients.Client(connectionId).enableSendButton();
        }

        public static void AgentJoinedChat(string name, string connectionId)
        {
            HubContext.Clients.Client(connectionId).agentJoined(name);
        }

        public static void UpdateAgentTyping(bool isTyping, string connectionId, string name)
        {
            HubContext.Clients.Client(connectionId).agentTyping(isTyping, name);
        }

        public static void Disconnect(string connectionId, DisconnectReason reason, string agentName = "", bool disconnect = true, bool duplicate = false)
        {
            if (reason == DisconnectReason.AgentDisconnect)
            {
                var webChat = WebChats.FirstOrDefault(w => w.ConnectionId == connectionId);
                if (webChat != null)
                {
                    var agent = !String.IsNullOrWhiteSpace(agentName) ? agentName : webChat.AgentName;
                    SendCustomSystemMessage(CustomMessageType.AgentDisconnect, agent, connectionId, webChat);
                    if (webChat.PreviousConnectionIds != null && webChat.PreviousConnectionIds.Any())
                    {
                        foreach (var cid in webChat.PreviousConnectionIds.Where(p => p != connectionId))
                        {
                            SendCustomSystemMessage(CustomMessageType.AgentDisconnect, webChat.AgentName, cid, webChat, true);
                        }
                    }
                }
            }
            if (reason == DisconnectReason.InactivityTimeout || reason == DisconnectReason.ContinueChatTimeout)
            {
                var webChat = WebChats.Any(i => i.ConnectionId == connectionId) ? WebChats.FirstOrDefault(w => w.ConnectionId == connectionId) : DisconnectedWebChats.FirstOrDefault(w => w.ConnectionId == connectionId);
                if (webChat != null)
                {
                    SendCustomSystemMessage(CustomMessageType.InactiveDisconnect, null, connectionId, webChat, duplicate);
                }
            }
            //if (reason == DisconnectReason.ContinueChatTimeout)
            //{
            //    SendCustomSystemMessage(CustomMessageType.InactiveDisconnect, null, connectionId);
            //}
            if (reason == DisconnectReason.SystemDisconnect)
            {
                HubContext.Clients.Client(connectionId).addNewSystemMessageToPage("Chat Disconnected");
                var webChat = WebChats.FirstOrDefault(w => w.ConnectionId == connectionId);
                if (webChat != null)
                {
                    if (webChat.PreviousConnectionIds != null && webChat.PreviousConnectionIds.Any())
                    {
                        foreach (var cid in webChat.PreviousConnectionIds.Where(p => p != connectionId))
                        {
                            HubContext.Clients.Client(cid).addNewSystemMessageToPage("Chat Disconnected");
                        }
                    }
                }
            }
            if (disconnect)
            {
                HubContext.Clients.Client(connectionId).disconnected(ChatController.EnableKeepOpenOnDisconnectAndStartNew(), false);
            }
            //HubContext.Clients.Client(connectionId).disconnected(ChatController.EnableKeepOpenOnDisconnectAndStartNew() || (reason == DisconnectReason.AgentDisconnect && ChatController.TransferTimeout > 0), false);
        }

        public static void ProcessDisconnect(string connectionId, bool agentDisconnect = false, bool force = false, string agentName = "")
        {
            try
            {
                if (agentDisconnect)
                {
                    Disconnect(connectionId, DisconnectReason.AgentDisconnect, agentName);
                }
                var repository = new Repository();
                var webChat = WebChats.FirstOrDefault(c => c.ConnectionId == connectionId);

                if (webChat != null)
                {
                    if (ChatController.ContinueChat() && !agentDisconnect && !force)
                    {
                        webChat.DatePaused = DateTime.Now;
                        if (webChat.State != ChatState.Queued)
                        {
                            webChat.State = ChatState.Paused;
                            SendCustomSystemMessage(CustomMessageType.PausedChat, webChat.UserName, connectionId, webChat);   
                        }
                    }
                    else
                    {
                        var chat = repository.Chats.Find(webChat.ChatId);
                        if (chat != null)
                        {
                            if (!agentDisconnect || force)
                            {
                                EndChat(chat.ParticipantId);
                            }
                            chat.DateEnded = DateTime.Now;
                            repository.SaveChanges();
                        }
                        if (webChat.DateAnswered.HasValue)//ChatController.EnableKeepOpenOnDisconnectAndStartNew() && webChat.DateAnswered.HasValue)
                        {
                            if (webChat.Messages.Any() && !DisconnectedWebChats.Any(
                                    d =>
                                        d.ConnectionId == webChat.ConnectionId ||
                                        (d.PreviousConnectionIds != null &&
                                         d.PreviousConnectionIds.Contains(webChat.ConnectionId))))
                            {
                                DisconnectedWebChats.Add(webChat);
                            }
                        }

                        WebChats.Remove(webChat);
                        //if (agentDisconnect && !force && ChatController.TransferTimeout > 0)
                        //{
                        //    webChat.State = ChatState.Pending;
                        //}
                        //else
                        //{
                        //    WebChats.Remove(webChat);
                        //}
                    }

                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static void SwitchoverDetected()
        {
            LoggingService.GetInstance().LogNote("Switchover Detected!!!");
            HubContext.Clients.All.serverReconnecting();
        }

        public static void ReconnectToCic(string connectionId)
        {
            HubContext.Clients.Client(connectionId).serverReconnected();
        }

        #region ChatWebSvcs

        public static bool GetConfig(string baseUrl = "")
        {
            GettingConfig = true;
            if (String.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = ConfigBaseUrl;
            }
            if (String.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = BaseUrl;
                if (String.IsNullOrWhiteSpace(baseUrl))
                {
                    return false;
                }
            }
            LoggingService.GetInstance().LogNote("GetConfig - baseUrl: " + baseUrl);
            var fullUrl = baseUrl + "/websvcs/serverConfiguration";
            var success = true;
            try
            {
                //var request = WebRequest.Create(fullUrl);
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                try
                {
                    var svp = ServicePointManager.FindServicePoint(new Uri(baseUrl));
                    var svcPointSettings = String.Format("ServicePointSettingsCIC: Nagle={0}|ConnectionLimit={1}", svp.UseNagleAlgorithm, svp.ConnectionLimit);
                    LoggingService.GetInstance().LogNote(svcPointSettings);
                }
                catch (Exception e)
                {
                }

                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                using (var response = request.GetResponse()) 
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
                success = false;
            }
            GettingConfig = false;
            return success;
        }

        public static Chat StartChat(ChatRequest chatRequest)
        {
            try
            {
                string fullUrl = BaseUrl + "/websvcs/chat/start";

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";
                
                var json = JsonConvert.SerializeObject(chatRequest);

                using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(json);
                }
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            var reJson = JsonConvert.DeserializeObject<ChatResponse>(rawJson);
                            return reJson.Chat;
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return null;
        }

        public static bool EndChat(string participantId)
        {
            var success = true;
            try
            {
                var fullUrl = BaseUrl + "/webSvcs/chat/exit/" + participantId;

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
                success = false;
            }
            return success;
        }

        public static void GetPartyInfo(long chatId, string visitorParticipantId, string agentParticipantId)
        {
            var fullUrl = BaseUrl + "/webSvcs/partyInfo/" + visitorParticipantId;
            try
            {
                var chat = WebChats.FirstOrDefault(i => i.ChatId == chatId);
                if (chat != null)
                {
                    var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                    request.KeepAlive = false;
                    request.Method = "POST";
                    request.ContentType = "application/json";

                    var json = JsonConvert.SerializeObject(new {participantId = agentParticipantId});

                    using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        requestWriter.Write(json);
                    }
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                var rawJson = reader.ReadToEnd();
                                if (!String.IsNullOrWhiteSpace(rawJson))
                                {
                                    LoggingService.GetInstance().LogNote("::Session Id::" + chat.SessionId + "::" + rawJson);
                                }
                                PartyInfo reJson = null;
                                try
                                {
                                    reJson = JsonConvert.DeserializeObject<PartyInfo>(rawJson);
                                }
                                catch (Exception e)
                                {
                                    LoggingService.GetInstance().LogException(e);
                                }

                                if (reJson != null)
                                {
                                    if (!String.IsNullOrWhiteSpace(reJson.Name))
                                    {
                                        if (chat.AgentName != reJson.Name)
                                        {
                                            chat.AgentName = reJson.Name;
                                        }
                                    }
                                }
                                else
                                {
                                    chat.AgentName = ChatController.GetAgentName();
                                }
                            }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static void PollEvents(string participantId, string connectionId, string sessionId)
        {
            try
            {
                var fullUrl = BaseUrl + "/WebSvcs/chat/poll/" + participantId;

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            if (!String.IsNullOrWhiteSpace(rawJson))
                            {
                                LoggingService.GetInstance().LogNote("::Session Id::" + sessionId + "::" + rawJson);
                            }
                            var reJson = JsonConvert.DeserializeObject<PollResponse>(rawJson);
                            if (reJson.Chat.Status.Type == "success")
                            {
                                var events = reJson.Chat.Events.OrderBy(e => e.SequenceNumber);
                                var processEvents = true;
                                var webChat = WebChats.FirstOrDefault(w => w.ConnectionId == connectionId || (w.PreviousConnectionIds != null && w.PreviousConnectionIds.Contains(connectionId)));
                                if (webChat != null)
                                {
                                    if (webChat.State == ChatState.Pending)
                                    {
                                        var agentEvent = events.FirstOrDefault(i => i.Type == "participantStateChanged" && i.ParticipantType == "Agent" && i.State == "active");
                                        if (agentEvent != null)
                                        {
                                            processEvents = false;
                                            ProcessPollEvent(agentEvent, connectionId);
                                            foreach (var @event in events.Where(i => i.Type != "participantStateChanged"))
                                            {
                                                ProcessPollEvent(@event, connectionId);
                                            }
                                        }
                                    }
                                    else if (webChat.State == ChatState.Connected || webChat.State == ChatState.Paused)
                                    {
                                        var agentEvent = events.FirstOrDefault(i => i.Type == "participantStateChanged" && i.ParticipantType == "Agent" && i.State == "disconnected");
                                        var sysEvent = events.FirstOrDefault(i => i.Type == "participantStateChanged" && i.ParticipantType == "System" && i.State == "active");
                                        if (agentEvent != null && sysEvent != null)
                                        {
                                            webChat.State = ChatState.Pending;
                                            processEvents = false;
                                            var agentName = ChatController.GetAgentName();
                                            var overrideAgent = ChatController.OverrideAgent();
                                            if (!overrideAgent)
                                            {
                                                agentName = agentEvent.ParticipantName;
                                            }
                                            //Transfer Message
                                            Disconnect(connectionId, DisconnectReason.AgentDisconnect, agentName, false);
                                            foreach (var @event in events.Where(i => i.Type != "participantStateChanged"))
                                            {
                                                ProcessPollEvent(@event, connectionId);
                                            }
                                        }
                                        var agentJoined = events.FirstOrDefault(i => i.Type == "participantStateChanged" && i.ParticipantType == "Agent" && i.State == "active");
                                        if (agentEvent != null && agentJoined != null)
                                        {
                                            processEvents = false;
                                            var agentDefault = ChatController.GetAgentName();
                                            var overrideAgent = ChatController.OverrideAgent();
                                            var agentX = agentDefault;
                                            var agentJ = agentDefault;
                                            if (!overrideAgent)
                                            {
                                                agentX = agentEvent.ParticipantName;
                                                agentJ = agentJoined.ParticipantName;
                                            }
                                            Disconnect(connectionId, DisconnectReason.AgentDisconnect, agentX, false);
                                            ProcessPollEvent(agentJoined, connectionId);
                                            foreach (var @event in events.Where(i => i.Type != "participantStateChanged"))
                                            {
                                                ProcessPollEvent(@event, connectionId);
                                            }
                                        }
                                    }
                                }

                                if (processEvents)
                                {
                                    var processed = new List<Event>();
                                    foreach (var @event in events)
                                    {
                                        //LoggingService.GetInstance().LogToFile(String.Format("Processing Event - Type: {0} ParticipantType: {1} ParticipantName: {2} DisplayName: {3} State: {4}", @event.Type, @event.ParticipantType, @event.ParticipantName, @event.DisplayName, @event.State), FolderNameSafeDate(), true);
                                        if (@event.Type != "participantStateChanged" || !processed.Any(e => e.ParticipantID == @event.ParticipantID && e.State == @event.State && e.Type == "participantStateChanged" && e.ParticipantType.ToLower() == "agent"))
                                        {
                                            processed.Add(@event);
                                            ProcessPollEvent(@event, connectionId);
                                        }
                                    }
                                }
                            }
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static void PollVisitorMessageEvents(VisitorMessageChat visitorMessageChat)
        {
            try
            {
                var participantId = visitorMessageChat.ParticipantId;
                var fullUrl = BaseUrl + "/WebSvcs/chat/poll/" + participantId;
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            var reJson = JsonConvert.DeserializeObject<PollResponse>(rawJson);
                            //LoggingService.GetInstance().LogToFile(String.Format("Visitor Message Polling Response: {0}", rawJson), FolderNameSafeDate(), true);
                            if (reJson.Chat.Status.Type == "success")
                            {
                                var events = reJson.Chat.Events.OrderBy(e => e.SequenceNumber);
                                foreach (var @event in events)
                                {
                                    if (@event.Type == "text")
                                    {
                                        var textValue = @event.Value as string;
                                        var type = @event.ParticipantType;
                                        if (!String.IsNullOrWhiteSpace(textValue) && String.Equals(type, "System", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (visitorMessageChat.State == VisitorMessageState.VoicemailSent)
                                            {
                                                EndChat(participantId);
                                                VisitorMessageChats.Remove(visitorMessageChat);
                                                var repository = new Repository();
                                                var vm = repository.VisitorMessages.FirstOrDefault(v => v.VisitorMessageId == visitorMessageChat.VisitorMessageId);
                                                if (vm != null)
                                                {
                                                    vm.IsProcessed = true;
                                                    vm.Type = VisitorMessageType.Voicemail;
                                                    repository.SaveChanges();
                                                }
                                            }
                                            if (visitorMessageChat.State == VisitorMessageState.InVoicemail)
                                            {
                                                var msg = visitorMessageChat.Message.Replace("|", @"\r\n");
                                                SendMessage(participantId, msg);
                                                visitorMessageChat.State = VisitorMessageState.VoicemailSent;
                                            }
                                            if (visitorMessageChat.State == VisitorMessageState.InConfirmation)
                                            {
                                                SendMessage(participantId, "Yes");
                                                visitorMessageChat.State = VisitorMessageState.InVoicemail;
                                            }
                                            if (visitorMessageChat.State == VisitorMessageState.Connected)
                                            {
                                                if (textValue.ToLower().Contains("voicemail"))
                                                {
                                                    SendMessage(participantId, "Yes");
                                                    visitorMessageChat.State = ChatController.ConfirmNameInVisitorMessagesVoicemail() ? VisitorMessageState.InConfirmation : VisitorMessageState.InVoicemail;
                                                }
                                            }
                                            //LoggingService.GetInstance().LogToFile(String.Format("VM Participant: {0} State: {1}", participantId, visitorMessageChat.State), FolderNameSafeDate(), true);
                                        }
                                    }
                                }
                            }
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static void ProcessPollEvent(Event @event, string connectionId)
        {
            var filePath = "";
            var agentName = ChatController.GetAgentName();
            var overrideAgent = ChatController.OverrideAgent();
            switch (@event.Type)
            {
                case "participantStateChanged":
                    if (String.Equals(@event.ParticipantType,"Agent", StringComparison.OrdinalIgnoreCase))
                    {
                        if (String.Equals(@event.State, "active", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!overrideAgent)
                            {
                                agentName = @event.ParticipantName;
                            }
                            //LoggingService.GetInstance().LogToFile(String.Format("Agent Joined - ConnectionId: {0} AgentName: {1}", connectionId, agentName), FolderNameSafeDate(), true);
                            var webChat = WebChats.FirstOrDefault(w => w.ConnectionId == connectionId || (w.PreviousConnectionIds != null && w.PreviousConnectionIds.Contains(connectionId)));
                            if (webChat != null)
                            {
                                var lastMessage = webChat.Messages.Where(i => i.Name.Equals("System", StringComparison.OrdinalIgnoreCase)).OrderBy(i => i.Order).LastOrDefault();
                                if (lastMessage != null)
                                {
                                    var elapsed = DateTime.Now.Subtract(lastMessage.DateSent).TotalSeconds;
                                    if (webChat.AgentName == agentName && elapsed < 5)
                                    {
                                        return;
                                    }
                                }
                                SendCustomSystemMessage(CustomMessageType.AgentJoined, agentName, connectionId, webChat);
                                AgentJoinedChat(agentName, connectionId);
                                try
                                {
                                    var queuedMessages = QueuedMessages.Where(m => m.ConnectionId == connectionId && !m.Sent).OrderBy(i => i.Order);
                                    foreach (var queuedMessage in queuedMessages)
                                    {
                                        SendMessage(webChat, connectionId, queuedMessage.Name, queuedMessage.Text);
                                        queuedMessage.Sent = true;
                                        queuedMessage.DateSent = DateTime.Now;
                                    }
                                }
                                catch (Exception e)
                                {
                                    LoggingService.GetInstance().LogException(e);
                                }
                                if (UnansweredChats.Any(u => u.SessionId == webChat.SessionId))
                                {
                                    var unanswered = UnansweredChats.FirstOrDefault(u => u.SessionId == webChat.SessionId);
                                    UnansweredChats.Remove(unanswered);
                                }
                                if (webChat.PreviousConnectionIds != null && webChat.PreviousConnectionIds.Any())
                                {
                                    foreach (var cid in webChat.PreviousConnectionIds.Where(p => p != connectionId))
                                    {
                                        SendCustomSystemMessage(CustomMessageType.AgentJoined, agentName, cid, webChat, true);
                                        AgentJoinedChat(agentName, cid);
                                    }
                                }
                                var repository = new Repository();
                                webChat.State = ChatState.Connected;
                                webChat.DateAnswered = DateTime.Now;
                                webChat.AgentName = agentName;
                                webChat.AgentParticipantId = @event.ParticipantID;

                                var chat = repository.Chats.Find(webChat.ChatId);
                                if (chat != null)
                                {
                                    chat.DateAnswered = DateTime.Now;
                                    repository.SaveChanges();
                                }
                            }
                        }
                        if (String.Equals(@event.State, "disconnected", StringComparison.OrdinalIgnoreCase))
                        {
                            //custom agent disconnected message here
                            if (!overrideAgent)
                            {
                                agentName = @event.ParticipantName;
                            }
                            ProcessDisconnect(connectionId,true,false,agentName);
                        }
                    }
                    else//if (String.Equals(@event.ParticipantType, "WebUser", StringComparison.OrdinalIgnoreCase))
                    {
                        if (String.Equals(@event.State, "disconnected", StringComparison.OrdinalIgnoreCase))
                        {
                            var webChat = WebChats.FirstOrDefault(c => c.ConnectionId == connectionId);
                            if (webChat != null && webChat.State == ChatState.Queued)
                            {
                                ProcessDisconnect(connectionId, false, true);
                                Disconnect(connectionId, DisconnectReason.SystemDisconnect); 
                            }
                        }
                    }
                    break;
                case "text":
                    var textValue = @event.Value as string;
                    var type = @event.ParticipantType;
                    if (!overrideAgent)
                    {
                        agentName = @event.DisplayName;
                    }
                    if (String.Equals(type, "System", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!ChatController.BlockSystemMessages() || (textValue != null && textValue.Contains("%FORCESEND%")))
                        {
                            if (textValue != null)
                            {
                                if (textValue.Contains("%ENABLESEND%"))
                                {
                                    textValue = textValue.Replace("%ENABLESEND%", "");
                                    EnableSendButton(connectionId);
                                }
                                textValue = textValue.Replace("%FORCESEND%", "");
                                Receive("", textValue, "", connectionId, true);
                            }
                        }
                    }
                    else if(String.Equals(type, "Agent", StringComparison.OrdinalIgnoreCase))
                    {
                        //Lookup agent photo
                        try
                        {
                            if (File.Exists(HostingEnvironment.MapPath("~/Images/Agents/" + agentName + ".png")))
                            {
                                filePath = "/Images/Agents/" + agentName + ".png";
                            }
                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                        Receive(agentName, textValue, filePath, connectionId);
                    }
                    break;
                case "file":
                    var fileValue = @event.Value as string;
                    if (!overrideAgent)
                    {
                        agentName = @event.DisplayName;
                    }
                    try
                    {
                        if (File.Exists(HostingEnvironment.MapPath("~/Images/Agents/" + agentName + ".png")))
                        {
                            filePath = "/Images/Agents/" + agentName + ".png";
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingService.GetInstance().LogException(e);
                    }
                    var wc = WebChats.FirstOrDefault(w => w.ConnectionId == connectionId);
                    if (wc != null)
                    {
                        if (!wc.AllowAttachments)
                        {
                            SendMessage(wc.ParticipantId, "Attachments are disabled for this chat. Your file was not sent.");
                            return;
                        }
                        DownloadFile(agentName, fileValue, filePath, connectionId, wc.SessionId);
                    }
                    break;
                case "url":
                    var urlValue = @event.Value as string;
                    if (!overrideAgent)
                    {
                        agentName = @event.DisplayName;
                    }
                    //Lookup agent photo
                    try
                    {
                        if (File.Exists(HostingEnvironment.MapPath("~/Images/Agents/" + agentName + ".png")))
                        {
                            filePath = "/Images/Agents/" + agentName + ".png";
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingService.GetInstance().LogException(e);
                    }
                    Receive(agentName, urlValue, filePath, connectionId);
                    break;
                case "typingIndicator":
                    var typingValue = @event.Value as bool?;
                    if (typingValue.HasValue)
                    {
                        var wct = WebChats.FirstOrDefault(w => w.ConnectionId == connectionId);
                        if (wct != null)
                        {
                            var agentDisplayName = !String.IsNullOrWhiteSpace(wct.AgentName) ? wct.AgentName : agentName;
                            UpdateAgentTyping(typingValue.Value, connectionId, agentDisplayName);
                            if (wct.PreviousConnectionIds != null && wct.PreviousConnectionIds.Any())
                            {
                                foreach (var previousConnectionId in wct.PreviousConnectionIds.Where(p => p != connectionId))
                                {
                                    UpdateAgentTyping(typingValue.Value, previousConnectionId, agentDisplayName);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public static string GetNewMessageId(long chatId = -1)
        {
            return String.Format("{0}-{1}", chatId, Guid.NewGuid().ToString().Replace("-", ""));
        }

        public static void DownloadFile(string agentName, string fileUrl, string agentPhotoPath, string connectionId, string sessionId)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var path = HostingEnvironment.MapPath("~/Files/" + sessionId);
                    if (path != null)
                    {
                        var exists = Directory.Exists(path);
                        if (!exists)
                        {
                            Directory.CreateDirectory(path);
                        }
                        var fileName = fileUrl.Split('/').Last();
                        path = Path.Combine(path, fileName);
                        client.DownloadFile(BaseUrl + fileUrl, path);
                        var protocol = "";
                        if (HttpContext.Current != null)
                        {
                            protocol = HttpContext.Current.Request.IsSecureConnection ? "https:" : "http:";
                        }
                        var downloadUrl = String.Format("{0}//{1}/Chat/DownloadFile?fileName={2}", protocol, ChatController.WcbDomain, HttpUtility.UrlEncode(fileName));
                        Receive(agentName, downloadUrl, agentPhotoPath, connectionId);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static bool SendMessage(string participantId, string message)
        {
            var success = true;
            try
            {
                var fullUrl = BaseUrl + "/WebSvcs/chat/sendMessage/" + participantId;

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(new {message, contentType = "text/plain" });

                using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(json);
                }

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
                success = false;
            }
            return success;
        }

        public static string FolderNameSafeDate()
        {
            return String.Format("{0:yyyy-MM-dd}", DateTime.Now);
        }

        public static void UpdateIndicator(string participantId, bool isTyping)
        {
            try
            {
                var fullUrl = BaseUrl + "/WebSvcs/chat/setTypingState/" + participantId;

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(new { typingIndicator = isTyping });

                using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(json);
                }

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public static bool ReconnectChat(string chatId)
        {
            var success = false;
            try
            {
                string fullUrl = BaseUrl + "/websvcs/chat/reconnect";

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(new {chatId });


                using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(json);
                }
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            var reJson = JsonConvert.DeserializeObject<ChatResponse>(rawJson);
                            if (reJson.Chat.Status.Type == "success")
                            {
                                success = true;
                            }
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
                success = false;
            }
            return success;
        }

        public static QueueAvailability QueryQueue(string queueName)
        {
            QueueAvailability availability = null;
            try
            {
                var fullUrl = BaseUrl + "/websvcs/queue/query";

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(new {queueName, queueType = "Workgroup", participant = new Participant{ Name = "Anonymous User", Credentials = null } });

                using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(json);
                }
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            var reJson = JsonConvert.DeserializeObject<QueryResponse>(rawJson);
                            if (reJson.Queue.Status.Type == "success")
                            {
                                availability = new QueueAvailability
                                {
                                    AgentsAvailable = reJson.Queue.AgentsAvailable,
                                    EstimatedWaitTime = reJson.Queue.EstimatedWaitTime
                                };
                            }
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return availability;
        }

#endregion

        #region CallBackWebSvcs

        public static Callback CreateCallback(CallbackRequest callbackRequest)
        {
            try
            {
                string fullUrl = BaseUrl + "/websvcs/callback/create";

                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(callbackRequest);

                using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                {
                    requestWriter.Write(json);
                }
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            var reJson = JsonConvert.DeserializeObject<CallbackResponse>(rawJson);
                            return reJson.Callback;
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return null;
        }

        #endregion

        #region WcbWindowsService

        public static void CicServer()
        {
            var lastResponse = _cicServer ?? "";
            try
            {
                var fullUrl = LocalServiceUrl + "GetCicServer";
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            _cicServer = JsonConvert.DeserializeObject<string>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
                _cicServer = "";
            }
            if (lastResponse != _cicServer)
            {
                LoggingService.GetInstance().LogWcbResponse("CicServer", _cicServer);
            }
        }

        public static string ServiceLog()
        {
            var log = "";
            try
            {
                var fullUrl = LocalServiceUrl + "GetLog";
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            log = JsonConvert.DeserializeObject<string>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return log;
        }

        public static int AgentsAvailable(int profileId)
        {
            var agentsAvailable = 0;
            try
            {
                var fullUrl = LocalServiceUrl + "AgentsAvailable?profileId=" + profileId;
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            agentsAvailable = JsonConvert.DeserializeObject<int>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            LoggingService.GetInstance().LogWcbResponse("AgentsAvailable", agentsAvailable.ToString());
            return agentsAvailable;
        }

        public static bool IsLicensed()
        {
            var success = true;
            try
            {
                var fullUrl = LocalServiceUrl + "IsLicensed";
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            success = JsonConvert.DeserializeObject<bool>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
                success = false;
            }
            LoggingService.GetInstance().LogWcbResponse("IsLicensed", success.ToString());
            return success;
        }

        public static DateTime? LastScheduleUpdate()
        {
            DateTime? lastUpdated = null;
            try
            {
                var fullUrl = LocalServiceUrl + "GetLastScheduleUpdate";
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            lastUpdated = JsonConvert.DeserializeObject<DateTime?>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            LoggingService.GetInstance().LogWcbResponse("LastScheduleUpdate", lastUpdated.HasValue ? lastUpdated.Value.ToString(CultureInfo.InvariantCulture) : "NULL");
            return lastUpdated;
        }

        public static bool UpdateRefreshWatch()
        {
            try
            {
                var fullUrl = LocalServiceUrl + "UpdateRefreshWatch";
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            return JsonConvert.DeserializeObject<bool>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return false;
        }

        public static bool RestartService()
        {
            LoggingService.GetInstance().LogNote("Issue Reported. Restarting service.");
            try
            {
                var fullUrl = LocalServiceUrl + "RestartService";
                var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Wcb-Sync", SyncKey.GetSyncValue());

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            var rawJson = reader.ReadToEnd();
                            return JsonConvert.DeserializeObject<bool>(rawJson);
                        }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return false;
        }

        #endregion

        //Responses

        public class ChatResponse
        {
            public Chat Chat { get; set; }
        }

        public class PollResponse
        {
            public Chat Chat { get; set; }
        }

        public class Chat
        {
            public int PollWaitSuggestion { get; set; }
            public string ParticipantID { get; set; }
            public string ChatID { get; set; }
            public string DateFormat { get; set; }
            public string TimeFormat { get; set; }
            public Status Status { get; set; }
            public ICollection<Event> Events { get; set; }
        }

        public class Callback
        {
            public string ParticipantID { get; set; }
            public string CallbackID { get; set; }
            public Status Status { get; set; }
        }

        public class CallbackResponse
        {
            public Callback Callback { get; set; }
        }

        public class Status
        {
            public string Type { get; set; }
            public string Reason { get; set; }
        }

        public class Event
        {
            public string Type { get; set; }
            public string ParticipantID { get; set; }
            public int SequenceNumber { get; set; }
            public string State { get; set; }
            public string ParticipantName { get; set; }
            public string ParticipantType { get; set; }
            public int? ConversationSequenceNumber { get; set; }
            public string ContentType { get; set; }
            public object Value { get; set; }
            public string DisplayName { get; set; }
        }

        public class PartyInfo
        {
            public string Name { get; set; }
            public Status Status { get; set; }
        }

        public class QueryResponse
        {
            public Queue Queue { get; set; }
        }

        public class Queue
        {
            public int AgentsAvailable { get; set; }
            public int EstimatedWaitTime { get; set; }
            public Status Status { get; set; }
        }

        public class QueueAvailability
        {
            public int AgentsAvailable { get; set; }
            public int EstimatedWaitTime { get; set; }
        }

        public enum DisconnectReason
        {
            AgentDisconnect,
            UserDisconnect,
            ContinueChatTimeout,
            InactivityTimeout,
            SystemDisconnect
        }
    }
}