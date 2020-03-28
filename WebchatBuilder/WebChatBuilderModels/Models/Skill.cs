using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WebChatBuilderModels.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        public string ConfigId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public virtual ICollection<Agent> AssignedAgents { get; set; }

        public virtual ICollection<Profile> ConfiguredProfiles { get; set; }

        [Required]
        public bool IsAssignable { get; set; }

        [Required]
        public bool MarkedForDeletion { get; set; }

        [NotMapped]
        public bool IsUsed
        {
            get { return ConfiguredProfiles != null && ConfiguredProfiles.Any(); }
        }
    }
}