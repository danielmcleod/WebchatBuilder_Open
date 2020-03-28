using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class WidgetSettingsViewModel
    {
        public ICollection<Widget> Widgets { get; set; }

        public ICollection<Form> Forms { get; set; }

        public bool ConfirmNameInVisitorMessages { get; set; }

        public bool UseProfileWorkgroupForVisitorMessages { get; set; }

        public string DefaultWorkgroupForVisitorMessages { get; set; }

        public bool UseCallbackForVisitorMessages { get; set; }

        public ICollection<string> Icons { get; set; }

    }
}