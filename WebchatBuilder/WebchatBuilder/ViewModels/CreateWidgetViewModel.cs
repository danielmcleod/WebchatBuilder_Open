using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebchatBuilder.ViewModels
{
    public class CreateWidgetViewModel
    {
        public int WidgetId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsSecondaryStyle { get; set; }

        public int RecycleTime { get; set; }

        public int StartTime { get; set; }

        public bool CheckForAgents { get; set; }

        public int RequiredAgentsAvailable { get; set; }

        public int MaxEstimatedWaitTime { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowTooltipOnMobile { get; set; }

        public int MobileWidth { get; set; }

        public bool AllowIframes { get; set; }

        public bool UseIframe { get; set; }

        public bool PopOverlay { get; set; }

        public bool UseIcon { get; set; }

        public string IconPath { get; set; }
        
        public string ResumeIconPath { get; set; }

        public string UnavailableIconPath { get; set; }

        public int IconWidth { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@.,?\s]+$", ErrorMessage = "Only letters, numbers, spaces, and these special characters !@,.? are allowed.")]
        public string LaunchText { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@.,?\s]+$", ErrorMessage = "Only letters, numbers, spaces, and these special characters !@,.? are allowed.")]
        public string ResumeLaunchText { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@.,?\s]+$", ErrorMessage = "Only letters, numbers, spaces, and these special characters !@,.? are allowed.")]
        public string UnavailableLaunchText { get; set; }

        public string Position { get; set; }

        public virtual ICollection<SelectListItem> PositionOptions
        {
            get
            {
                return new Collection<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Bottom",
                        Value = "bottom",
                        Selected = Position.ToLower() == "bottom"
                    },
                    new SelectListItem
                    {
                        Text = "Right",
                        Value = "right",
                        Selected = Position.ToLower() == "right"
                    },
                    new SelectListItem
                    {
                        Text = "Left",
                        Value = "left",
                        Selected = Position.ToLower() == "left"
                    }
                };
            }
        }

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

        [RegularExpression(@"^[a-zA-Z0-9!@.,?\s]+$", ErrorMessage = "Only letters, numbers, spaces, and these special characters !@,.? are allowed.")]
        public string TooltipText { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@.,?\s]+$", ErrorMessage = "Only letters, numbers, spaces, and these special characters !@,.? are allowed.")]
        public string ResumeTooltipText { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@.,?\s]+$", ErrorMessage = "Only letters, numbers, spaces, and these special characters !@,.? are allowed.")]
        public string UnavailableTooltipText { get; set; }

        public string TooltipColor { get; set; }

        public bool ShowTooltip { get; set; }

        public int ShowTooltipAtStart { get; set; }

        public bool LaunchInNewWindow { get; set; }

        public ICollection<SelectListItem> Forms { get; set; }

        public int FormId { get; set; }

        public ICollection<SelectListItem> UnavailableForms { get; set; }

        public int UnavailableFormId { get; set; }

        public bool ShowUnavailableIfOpenNoAgents { get; set; }

        //Image Collections
        public ICollection<string> Icons { get; set; }

    }
}