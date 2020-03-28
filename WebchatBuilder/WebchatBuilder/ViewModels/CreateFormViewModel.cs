using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class CreateFormViewModel
    {
        public int FormId { get; set; }

        [Required]
        public string FormName { get; set; }

        public virtual ICollection<FormField> FormFields { get; set; }

        public string BackgroundColor { get; set; }

        public string LabelColor { get; set; }

        public bool Rounded { get; set; }

        public string BorderColor { get; set; }

        public string ButtonColor { get; set; }

        public string ButtonTextColor { get; set; }

        [Required]
        public string ButtonText { get; set; }

        public bool ShowFormMessage { get; set; }

        public bool UseScheduleMessage { get; set; }

        public string FormMessage { get; set; }

        public string FormSubmittedMessage { get; set; }

        public ICollection<SelectListItem> FieldTypes { get; set; }
 
    }
}