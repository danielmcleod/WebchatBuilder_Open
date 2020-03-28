using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class FormOption
    {
        [Key]
        public int FormOptionId { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public string Text { get; set; }

        public bool IsDefault { get; set; }

        public virtual Profile Profile { get; set; }

        [NotMapped]
        public bool HasProfile
        {
            get
            {
                return Profile != null;
            }
        }
    }
}
