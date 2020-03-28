using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class CustomMessagesViewModel
    {
        public bool ConnectedMessageEnabled { get; set; }

        public bool AgentJoinedMessageEnabled { get; set; }

        public bool AgentDisconnectMessageEnabled { get; set; }

        public bool InactiveDisconnectMessageEnabled { get; set; }

        public bool RestartedChatMessageEnabled { get; set; }

        public bool PausedChatMessageEnabled { get; set; }

        public bool ResumedChatMessageEnabled { get; set; }

        public bool VisitorDisconnectMessageEnabled { get; set; }

        public string ConnectedMessage { get; set; }

        public string AgentJoinedMessage { get; set; }

        public string AgentDisconnectMessage { get; set; }

        public string InactiveDisconnectMessage{ get; set; }

        public string RestartedChatMessage { get; set; }

        public string PausedChatMessage { get; set; }

        public string ResumedChatMessage { get; set; }

        public string VisitorDisconnectMessage { get; set; }
    }
}