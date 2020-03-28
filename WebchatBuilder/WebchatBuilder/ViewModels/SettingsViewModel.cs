using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class SettingsViewModel
    {
        public GeneralSettingsViewModel GeneralSettingsViewModel { get; set; }

        public SkillsSettingsViewModel SkillsSettingsViewModel { get; set; }

        public WorkgroupsSettingsViewModel WorkgroupsSettingsViewModel { get; set; }

        public TemplateSettingsViewModel TemplateSettingsViewModel { get; set; }

        public WidgetSettingsViewModel WidgetSettingsViewModel { get; set; }

        public ScheduleSettingsViewModel ScheduleSettingsViewModel { get; set; }
    }
}