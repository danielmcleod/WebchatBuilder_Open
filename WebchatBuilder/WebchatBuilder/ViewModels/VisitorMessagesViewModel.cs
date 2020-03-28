using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.ViewModels
{
    public class VisitorMessagesViewModel
    {
        public ICollection<VisitorMessage> VisitorMessages { get; set; }

        public ICollection<VisitorMessage> ProcessedMessages { get; set; }
    }
}