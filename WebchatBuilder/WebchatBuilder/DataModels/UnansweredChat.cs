using System;
using System.Collections.ObjectModel;
using WebchatBuilder.Services;

namespace WebchatBuilder.DataModels
{
    public class UnansweredChat
    {
        public string SessionId { get; set; }

        public DateTime LastUpdated { get; set; }

        public virtual Collection<ChatMessage> Messages { get; set; } 
    }
}
