using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class Agent
    {
        [Key]
        public int AgentId { get; set; }

        [Required]
        public string ConfigId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string DisplayImage { get; set; }

        [Required]
        public bool HasActiveClientLicense { get; set; }

        [Required]
        public int MediaLevel { get; set; }

        [Required]
        public bool IsLicensedForChat { get; set; }

        public virtual ICollection<Utilization> Utilizations { get; set; }

        public virtual ICollection<Workgroup> ActiveInWorkgroups { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

    }
}
