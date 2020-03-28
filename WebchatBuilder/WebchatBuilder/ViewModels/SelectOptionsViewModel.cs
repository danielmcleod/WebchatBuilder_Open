using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class SelectOptionsViewModel
    {
        public int FieldId { get; set; }

        public bool IsProfileList { get; set; }

        public ICollection<string> Profiles { get; set; }

        public ICollection<FormOption> FormOptions { get; set; }
    }
}