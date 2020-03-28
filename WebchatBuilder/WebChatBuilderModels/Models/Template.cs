using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class Template
    {
        [Key]
        public int TemplateId { get; set; }

        [Required]
        public string Title { get; set; }

        public string CustomCss { get; set; }

        [Required]
        public bool IncludeHeader { get; set; }

        public string HeaderText { get; set; }

        public string HeaderLogoPath { get; set; }

        [Required]
        public bool HeaderIcons { get; set; }

        public bool UseUnstyledHeaderIcons { get; set; }

        public string CloseButtonIcon { get; set; }

        public string DisconnectButtonIcon { get; set; }

        public string PrintButtonIcon { get; set; }

        public string EmailButtonIcon { get; set; }

        [Required]
        public bool IncludeTranscript { get; set; }

        [Required]
        public bool IncludePrint { get; set; }

        [Required]
        public bool IncludeDisconnect { get; set; }

        public string PlaceholderText { get; set; }

        public string SendButtonIcon { get; set; }

        [Required]
        public bool SendIncludeIcon { get; set; }

        [Required]
        public bool ShowInitials { get; set; }

        [Required]
        public bool MessageArrows { get; set; }

        [Required]
        public bool ShowTime { get; set; }

        [Required]
        public bool HideHeader { get; set; }

        [Required]
        public bool HideSendButton { get; set; }
    }
}