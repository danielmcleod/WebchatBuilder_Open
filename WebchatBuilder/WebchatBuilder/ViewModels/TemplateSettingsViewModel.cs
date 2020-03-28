using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class TemplateSettingsViewModel
    {
        public ICollection<Template> Templates { get; set; }
    }
}