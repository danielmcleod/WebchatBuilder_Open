using WebchatBuilder.DataModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class DynamicChatScriptViewModel
    {
        public string FullDomain { get; set; }

        public string Domain { get; set; }

        public string DynamicHtml { get; set; }

        public string Profile { get; set; }

        public int RecycleTime { get; set; }

        public int StartTime { get; set; }

        public bool CheckForAgents { get; set; }

        public int RequiredAgentsAvailable { get; set; }

        public int MaxEstimatedWaitTime { get; set; }

        public bool ShowOnMobile { get; set; }

        public int MobileWidth { get; set; }

        public bool UseIframe { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public bool PopOverlay { get; set; }

        public bool ShowTooltip { get; set; }

        public bool ShowTooltipOnMobile { get; set; }

        public int ShowTooltipAtStart { get; set; }

        public bool LaunchInNewWindow { get; set; }

        public virtual Form Form { get; set; }

        public virtual Form UnavailableForm { get; set; }

        public int ContinueChatTimeRemaining { get; set; }

        public bool ContinueChat { get; set; }

        public string CustomCss { get; set; }

        public bool IncludeUserDataInCustomInfo { get; set; }

        public bool IncludeUserDataInAttributes { get; set; }

        public string UserData { get; set; }

        public bool IsSecondaryStyle { get; set; }

        public ChatState ChatState { get; set; }

        public bool IsChatMinimized { get; set; }

        public bool HasSchedules { get; set; }

        public bool ShowUnavailableIfOpenNoAgents { get; set; }
    }

    public class DynamicChatHtmlViewModel
    {
        public string Domain { get; set; }

        public bool PopOverlay { get; set; }

        public bool UseIcon { get; set; }

        public string IconPath { get; set; }

        public string ResumeIconPath { get; set; }

        public string UnavailableIconPath { get; set; }

        public int IconWidth { get; set; }

        public string LaunchText { get; set; }

        public string ResumeLaunchText { get; set; }

        public string UnavailableLaunchText { get; set; }

        public string Position { get; set; }

        public bool Rounded { get; set; }

        public bool Vertical { get; set; }

        public string Background { get; set; }

        public string TextColor { get; set; }

        public string PlaceHolderBackground { get; set; }

        public bool ShowLoader { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public bool ShowTooltip { get; set; }

        public string TooltipText { get; set; }

        public string ResumeTooltipText { get; set; }

        public string UnavailableTooltipText { get; set; }

        public string TooltipColor { get; set; }
    }
}