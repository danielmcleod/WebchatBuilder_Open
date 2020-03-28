using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class CreateProfileViewModel
    {
        public int ProfileId { get; set; }

        [Required]
        public string ProfileName { get; set; }

        public string Description { get; set; }

        public string HeaderText { get; set; }

        public string HeaderLogoPath { get; set; }

        public bool IncludeUserDataAsCustomInfo { get; set; }

        public bool IncludeUserDataAsAttributes { get; set; }

        [Required]
        public int Workgroup { get; set; }

        public virtual ICollection<SelectListItem> WorkgroupList { get; set; }

        [Required]
        public int Template { get; set; }

        public virtual ICollection<SelectListItem> TemplateList { get; set; }

        [Required]
        public int Widget { get; set; }

        public virtual ICollection<SelectListItem> WidgetList { get; set; }

        public virtual ICollection<int> Skills { get; set; }

        public virtual ICollection<Skill> SkillsList { get; set; }

        public virtual ICollection<int> Schedules { get; set; }

        public virtual ICollection<Schedule> SchedulesList { get; set; }

        //Image Collections
        public ICollection<string> Logos { get; set; }

        public string GeneratedScript { get; set; }

        public bool AllowAttachments { get; set; }
    }
}