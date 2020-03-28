using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using Microsoft.AspNet.SignalR;
using WebchatBuilder.Controllers;
using WebchatBuilder.DataModels;
using WebchatBuilder.Services;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.Hubs
{
    public class WebChatHub : Hub
    {
        public void Send(string name, string message)
        {
            try
            {
                var connectionId = Context.ConnectionId;

                var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                if (chat != null)
                {
                    if (chat.State != ChatState.Disconnected)
                    {
                        ChatServices.SendMessage(chat, connectionId, name, message);
                    }
                    else
                    {
                        ChatNotFound(message);
                        ChatServices.QueueMessageToAgent(connectionId, message, name);
                    }
                }
                else
                {
                    ChatNotFound(message);
                    ChatServices.QueueMessageToAgent(connectionId, message, name);
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        public void PauseChat()
        {
            var connectionId = Context.ConnectionId;
            var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null &&  c.PreviousConnectionIds.Contains(connectionId)));
            if (chat != null && chat.DateAnswered.HasValue && chat.State != ChatState.Disconnected)
            {
                chat.IsMinimized = true;
                chat.State = ChatState.Paused;
                chat.DatePaused = DateTime.Now;
                ChatServices.SendCustomSystemMessage(CustomMessageType.PausedChat, chat.UserName, connectionId, chat);
            }
        }

        public void ResumeChat()
        {
            var connectionId = Context.ConnectionId;
            var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
            if (chat != null && chat.State == ChatState.Paused)
            {
                chat.IsMinimized = false;
                chat.State = ChatState.Connected;
                ChatServices.SendCustomSystemMessage(CustomMessageType.ResumedChat, chat.UserName, connectionId, chat);
            }
        }

        public void SendCustomInfoToAgent(string customInfo)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
                if (chat != null && chat.State != ChatState.Disconnected && !String.IsNullOrWhiteSpace(customInfo))
                {
                    if (customInfo.Contains(";"))
                    {
                        var customInfoList = customInfo.Split(';');
                        foreach (var customInfoLine in customInfoList)
                        {
                            ChatServices.SendMessage(chat.ParticipantId, customInfoLine);
                        }
                    }
                    else
                    {
                        ChatServices.SendMessage(chat.ParticipantId, customInfo);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        private void ChatNotFound(string message)
        {
            LoggingService.GetInstance().LogNote("Chat Not Found!!!");
            if (ChatController.EnableKeepOpenOnDisconnectAndStartNew())
            {
                Clients.Caller.restartChat(message);
            }
            else
            {
                Clients.Caller.addNewSystemMessageToPage(ChatController.GetConnectionLostText());
            }
        }

        private static int TestModeInt = 0;

        public void SendTest(string name, string message)
        {
            var imgsrc = "/Content/Images/user.png";
            var direction = "out";
            if (TestModeInt == 0)
            {
                TestModeInt = 1;
            }
            else
            {
                TestModeInt = 0;
                direction = "in";
                name = "Agent";
                imgsrc = "/Content/Images/agent.png";
            }
            var initials = ChatServices.GetInitialsFromName(name);
            var messageId = ChatServices.GetNewMessageId();
            Clients.Caller.addNewMessageToPage(name, message, imgsrc, direction, initials, messageId);
        }

        public void Typing(bool isTyping)
        {
            var connectionId = Context.ConnectionId;
            var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));
            if (chat != null)
            {
                ChatServices.UpdateIndicator(chat.ParticipantId, isTyping);
            }
        }

        public void Disconnect(string connectionId, bool forceDisconnect)
        {
            var chat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId || (c.PreviousConnectionIds != null && c.PreviousConnectionIds.Contains(connectionId)));

            if (!forceDisconnect)
            {
                if (chat != null && ChatController.ContinueChat() && (chat.State == ChatState.Connected || chat.State == ChatState.Paused))
                {
                    var now = DateTime.Now;
                    var remaining = ChatController.ContinueChatTimeout();
                    if (chat.DateEnded.HasValue)
                    {
                        var elapsed = now.Subtract(chat.DateEnded.Value);
                        remaining = ChatController.ContinueChatTimeout() - Convert.ToInt32(elapsed.TotalSeconds);
                    }
                    if (remaining > 0)
                    {
                        var isPaused = chat.State == ChatState.Paused;
                        Clients.Client(connectionId).pauseWcbChat(isPaused);
                        return;
                    }
                }

                Clients.Client(connectionId).disconnected(ChatController.EnableKeepOpenOnDisconnectAndStartNew(), true);
            }
            else
            {
                if (chat != null)
                {
                    ChatServices.SendCustomSystemMessage(CustomMessageType.VisitorDisconnect, "", chat.ConnectionId, chat);
                    if (chat.PreviousConnectionIds != null && chat.PreviousConnectionIds.Any())
                    {
                        foreach (var cid in chat.PreviousConnectionIds.Where(p => p != chat.ConnectionId))
                        {
                            ChatServices.SendCustomSystemMessage(CustomMessageType.VisitorDisconnect, "", cid, chat);
                        }
                    }
                    ChatServices.ProcessDisconnect(chat.ConnectionId, false, true);
                }
            }
        }

        public override Task OnConnected()
        {
            var connectionId = Context.ConnectionId;
            LoggingService.GetInstance().LogNote(connectionId + " connected to WebChatHub");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                LoggingService.GetInstance().LogNote(connectionId + " disconnected from WebChatHub");
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.ConnectionId == connectionId);
                if (webChat != null && !webChat.DateAnswered.HasValue)
                {
                    var keepQueuedChatsAlive = ChatController.KeepQueuedChatsAlive;
                    if (keepQueuedChatsAlive)
                    {
                        ChatServices.ProcessDisconnect(connectionId);
                    }
                    else
                    {
                        if (webChat.Messages.Any() && ChatController.EnableReloadUnansweredChatHistory())
                        {
                            var unanswered = ChatServices.UnansweredChats.FirstOrDefault(u => u.SessionId == webChat.SessionId);
                            if (unanswered == null)
                            {
                                unanswered = new UnansweredChat
                                {
                                    LastUpdated = DateTime.Now,
                                    SessionId = webChat.SessionId,
                                    Messages = webChat.Messages
                                };
                                ChatServices.UnansweredChats.Add(unanswered);
                            }
                            else
                            {
                                foreach (var chatMessage in webChat.Messages)
                                {
                                    if (!unanswered.Messages.Any(u => u.Id == chatMessage.Id))
                                    {
                                        unanswered.Messages.Add(chatMessage);
                                    }
                                }
                                unanswered.LastUpdated = DateTime.Now;
                            }
                        }
                        ChatServices.ProcessDisconnect(connectionId, false, true);
                    }
                }
                else
                {
                    ChatServices.ProcessDisconnect(connectionId);                    
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}