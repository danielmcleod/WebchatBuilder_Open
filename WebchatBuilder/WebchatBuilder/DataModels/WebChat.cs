using System;
using System.Collections.ObjectModel;
using WebchatBuilder.Services;

namespace WebchatBuilder.DataModels
{
    public class WebChat
    {
        public long ChatId { get; set; }

        public string SessionId { get; set; }

        public string ConnectionId { get; set; }

        public Collection<String> PreviousConnectionIds { get; set; }

        public string ChatIdentifier { get; set; }

        public string ParticipantId { get; set; }

        public string UserName { get; set; }

        public string AgentName { get; set; }

        public string AgentParticipantId { get; set; }

        public string AgentPhoto { get; set; }

        public string ProfileName { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateAnswered { get; set; }

        public DateTime? DateEnded { get; set; }

        public DateTime? DatePaused { get; set; }

        public ChatState State { get; set; }

        public virtual ChatRequest ChatRequest { get; set; }

        public virtual Collection<ChatMessage> Messages { get; set; }

        public bool AllowAttachments { get; set; }

        public bool IsMinimized { get; set; }
    }

    public enum ChatState
    {
        Trying,
        Queued,
        Connected,
        Disconnected,
        Paused,
        Pending
    }

    public class ChatMessage
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public bool Sent { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string ImgSrc { get; set; }

        public string Direction { get; set; }

        public string Initials { get; set; }

        public string Id { get; set; }

        public string ConnectionId { get; set; }

    }
}
