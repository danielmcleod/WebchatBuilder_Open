using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class WorkgroupsSettingsViewModel
    {
        public ICollection<WorkgroupSettingViewModel> AssignableWorkgroups { get; set; }

        public ICollection<WorkgroupSettingViewModel> UnassignableWorkgroups { get; set; }
    }

    public class WorkgroupSettingViewModel
    {
        public string DisplayName { get; set; }

        public bool IsUsed { get; set; }

        public int WorkgroupId { get; set; }

    }
}