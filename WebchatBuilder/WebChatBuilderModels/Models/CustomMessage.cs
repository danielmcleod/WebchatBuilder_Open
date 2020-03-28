using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class CustomMessage
    {
        [Key]
        public int CustomMessageId { get; set; }

        public string Name { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        public string Message { get; set; }

        [Required]
        public virtual CustomMessageType Type { get; set; }

    }

public enum CustomMessageType
{
    Connected,
    AgentJoined,
    AgentDisconnect,
    InactiveDisconnect,
    RestartedChat,
    PausedChat,
    ResumedChat,
    VisitorDisconnect
}
}
