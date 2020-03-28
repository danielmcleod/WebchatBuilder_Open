using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class SkillsSettingsViewModel
    {
        public ICollection<SkillSettingsViewModel> AssignableSkills { get; set; }

        public ICollection<SkillSettingsViewModel> UnassignableSkills { get; set; }
    }

    public class SkillSettingsViewModel
    {
        public string DisplayName { get; set; }

        public bool IsUsed { get; set; }

        public int SkillId { get; set; }

    }
}