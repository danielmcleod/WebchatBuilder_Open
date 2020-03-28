using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }

        [Required]
        public string FormName { get; set; }

        public bool ShowFormMessage { get; set; }

        public bool UseScheduleMessage { get; set; }

        public string FormMessage { get; set; }

        public string FormSubmittedMessage { get; set; }

        public virtual ICollection<FormField> FormFields { get; set; }

        public string LabelColor { get; set; }

        public bool Rounded { get; set; }

        public string BackgroundColor { get; set; }

        public string BorderColor { get; set; }

        public string ButtonColor { get; set; }

        public string ButtonTextColor { get; set; }

        public string ButtonText { get; set; }

        [NotMapped]
        public bool HasRequired {
            get
            {
                return FormFields != null && FormFields.Any(f => f.IsRequired);
            } 
        }
    }
}
