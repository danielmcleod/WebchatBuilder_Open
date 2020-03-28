using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string HeaderText { get; set; }

        public string HeaderLogoPath { get; set; }

        public bool IncludeUserDataAsCustomInfo { get; set; }

        public bool IncludeUserDataAsAttributes { get; set; }

        public virtual Workgroup Workgroup { get; set; }

        //public virtual Language Language { get; set; }

        public virtual Template Template { get; set; }

        public virtual Widget Widget { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual ApplicationUser LastUpdatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }

        public bool AllowAttachments { get; set; }

    }
}