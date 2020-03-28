using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WebChatBuilderModels.Models
{
    public class Workgroup
    {
        [Key]
        public int WorkgroupId { get; set; }

        [Required]
        public string ConfigId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsAcd { get; set; }

        [Required]
        public bool HasQueue { get; set; }

        [Required]
        public bool IsAssignable { get; set; }

        [Required]
        public bool MarkedForDeletion { get; set; }

        public virtual ICollection<Agent> ActiveMembers { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public virtual ICollection<Utilization> Utilizations { get; set; }

        [NotMapped]
        public bool IsUsed
        {
            get { return Profiles != null && Profiles.Any(); }
        }
    }
}