using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class ProfileViewModel
    {
        public int ProfileId { get; set; }

        public string ProfileName { get; set; }

        public string Description { get; set; }

        public string Workgroup { get; set; }

        public string Template { get; set; }

        public string Widget { get; set; }

        public virtual ICollection<string> Skills { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedOn { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public virtual ICollection<string> Schedules { get; set; }
    }
}