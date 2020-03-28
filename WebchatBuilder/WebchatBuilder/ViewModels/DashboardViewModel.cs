using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebchatBuilder.ViewModels
{
    public class DashboardViewModel
    {
        public int ActiveChats { get; set; }

        public int QueuedChats { get; set; }

        public int TotalChats { get; set; }

        public int AbandonedChats { get; set; }

        public ICollection<string> Profiles { get; set; }
    }
}