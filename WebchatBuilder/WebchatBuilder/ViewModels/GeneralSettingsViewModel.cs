using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class GeneralSettingsViewModel
    {
        [Required(ErrorMessage = "Please Enter your Company Name")]
        public string CompanyName { get; set; }

        //[Required(ErrorMessage = "Please upload and/or Select a Logo")]
        public string Logo { get; set; }

        public ICollection<string> Logos { get; set; }

        public bool SaveIpAddress { get; set; }
        public string DefaultUserName { get; set; }
        public bool ContinueChat { get; set; }
        public int ChatTimeout { get; set; }
        public bool Iframes { get; set; }
        public string DefaultAgentName { get; set; }
        public bool OverrideAgentName { get; set; }
        public bool EnableSendAndQueue { get; set; }
        public bool DropInactiveChats { get; set; }
        public bool ResetActivityTimeoutOnAgentMessage { get; set; }
        public int InactiveChatTimeout { get; set; }
        public bool BlockCicSystemMessages { get; set; }
        public bool UseCustomSystemMessages { get; set; }
        public bool KeepOpenOnDisconnectAndStartNew { get; set; }
        public int TransferTimeout { get; set; }
        public string CustomErrorText { get; set; }
        public bool PassHistoryToNewAgentOnRestart { get; set; }
        public bool ReloadUserHistoryOnNewChat { get; set; }
        public bool ReloadUnansweredChatHistory { get; set; }
        public bool LoggingEnabled { get; set; }
        public string ConnectionLostText { get; set; }
        public bool BrowserAlertsEnabled { get; set; }
        public bool AudioAlertsEnabled { get; set; }
        public bool ShowOptionsButton { get; set; }
        //public bool EnableHardDisconnect { get; set; }
        public bool ShowUserAgentInUserData { get; set; }
        public string CloseButtonTitle { get; set; }
        public string DisconnectButtonTitle { get; set; }
        public string EmailTranscriptSubject { get; set; }
        public bool KeepQueuedChatsAlive { get; set; }
        public bool ShowCustomInfoOnReload { get; set; }
        public bool ShowCustomInfoOnLoad { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
        public bool EnableGoogleAnalytics { get; set; }
    }
}