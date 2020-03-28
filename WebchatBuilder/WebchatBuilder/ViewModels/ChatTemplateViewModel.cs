using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebchatBuilder.ViewModels
{
    public class ChatTemplateViewModel
    {
        public string Profile { get; set; }

        public string Title { get; set; }

        public string CustomCss { get; set; }

        public bool IncludeHeader { get; set; }

        public string HeaderText { get; set; }

        public string HeaderLogoPath { get; set; }

        public bool HeaderIcons { get; set; }

        public bool IncludeTranscript { get; set; }

        public bool IncludePrint { get; set; }

        public bool IncludeDisconnect { get; set; }

        public string PlaceholderText { get; set; }

        public string SendButtonIcon { get; set; }

        public bool SendIncludeIcon { get; set; }

        public bool ShowInitials { get; set; }

        public bool MessageArrows { get; set; }

        public bool ShowTime { get; set; }

        public string UserName { get; set; }

        public bool TestMode { get; set; }

        public string ErrorText { get; set; }

        public string Domain { get; set; }

        public string SessionId { get; set; }

        public string DefaultAgentName { get; set; }

        public bool OverrideAgentName { get; set; }

        public bool EnableSendAndQueueChatsBeforeAgent { get; set; }

        public bool HideHeader { get; set; }

        public bool HideSendButton { get; set; }

        public bool UseUnstyledHeaderIcons { get; set; }

        public string CloseButtonIconPath { get; set; }

        public string DisconnectButtonIconPath { get; set; }

        public string PrintButtonIconPath { get; set; }

        public string EmailButtonIconPath { get; set; }

        public string CloseButtonTitle { get; set; }

        public string DisconnectButtonTitle { get; set; }

        public string LaunchTextOverride { get; set; }

        public string TooltipOverrideText { get; set; }

        public string LaunchIconOverridePath { get; set; }

        public bool EnableAudioAlerts { get; set; }

        public bool EnableBrowserAlerts { get; set; }

        public bool ShowOptionsButton { get; set; }

        public bool ShowCustomInfoOnReload { get; set; }

        public bool ShowCustomInfoOnLoad { get; set; }

        //public bool ShowHardDisconnect { get; set; }

    }
}