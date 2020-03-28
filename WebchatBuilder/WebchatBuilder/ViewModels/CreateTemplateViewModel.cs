using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebchatBuilder.ViewModels
{
    public class CreateTemplateViewModel
    {
        [Required]
        public int TemplateId { get; set; }

        [Required]
        public string Title { get; set; }

        public string CustomCss { get; set; }

        public bool IncludeHeader { get; set; }

        public string HeaderText { get; set; }

        public string HeaderLogoPath { get; set; }

        public bool HeaderIcons { get; set; }

        public bool UseUnstyledHeaderIcons { get; set; }

        public string CloseButtonIcon { get; set; }

        public string DisconnectButtonIcon { get; set; }

        public string PrintButtonIcon { get; set; }

        public string EmailButtonIcon { get; set; }

        public bool IncludeTranscript { get; set; }

        public bool IncludePrint { get; set; }

        public bool IncludeDisconnect { get; set; }

        public string PlaceholderText { get; set; }

        public string SendButtonIcon { get; set; }

        public bool SendIncludeIcon { get; set; }

        public bool ShowInitials { get; set; }

        public bool MessageArrows { get; set; }

        public bool ShowTime { get; set; }

        public bool HideHeader { get; set; }

        public bool RoundHeader { get; set; }

        public bool HideSendButton { get; set; }

        //Image Collections
        public ICollection<string> Logos { get; set; }

        public ICollection<string> SendIcons { get; set; }

        public ICollection<string> CloseButtonIcons { get; set; }

        public ICollection<string> HeaderButtonIcons { get; set; }

        //CSS Defaults
        public string BackgroundColor { get; set; }

        public string HeaderFontColor { get; set; }

        public bool RoundImages { get; set; }

        public string ServerBackgroundColor { get; set; }

        public string ServerFontColor { get; set; }

        public string ServerSeparatorColor { get; set; }

        public string ImageBackgroundColor { get; set; }

        public string ImageBorderColor { get; set; }

        public string InitialsFontColor { get; set; }

        public string VisitorBackgroundColor { get; set; }

        public string VisitorBorderColor { get; set; }

        public string VisitorFontColor { get; set; }

        public string VisitorLinkColor { get; set; }

        public string VisitorNameColor { get; set; }

        public string AgentBackgroundColor { get; set; }

        public string AgentBorderColor { get; set; }

        public string AgentFontColor { get; set; }

        public string AgentLinkColor { get; set; }

        public string AgentNameColor { get; set; }

        public string AgentTypingBackgroundColor { get; set; }

        public string AgentTypingFontColor { get; set; }

        public bool SimplifyTemplate { get; set; }

        //Preview Options
        public int PreviewWidth { get; set; }

        public int PreviewHeight { get; set; }

    }
}