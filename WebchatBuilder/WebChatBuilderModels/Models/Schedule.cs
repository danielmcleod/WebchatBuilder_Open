using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ConfigId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string OverrideMessage { get; set; }

        public bool IsActive { get; set; }

        public string Keywords { get; set; }

        public bool ClosedOnly { get; set; }

        public virtual ScheduleRecurrence ScheduleRecurrence { get; set; }

        public DateTime DateLastModified { get; set; }

        [Required]
        public bool IsAssignable { get; set; }

        [Required]
        public bool MarkedForDeletion { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        [NotMapped]
        public bool IsUsed
        {
            get { return Profiles != null && Profiles.Any(); }
        }
    }
}
