using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebchatBuilder.ViewModels
{
    public class ScheduleSettingsViewModel
    {
        public ICollection<ScheduleListViewModel> Schedules { get; set; }
    }

    public class ScheduleListViewModel
    {
        public int ScheduleId { get; set; }

        public string Name { get; set; }

        public bool IsAssignable { get; set; }

        public bool MarkedForDeletion { get; set; }

        public bool ClosedOnly { get; set; }

        public bool IsUsed { get; set; }
    }

    public class EditScheduleViewModel
    {
        public int ScheduleId { get; set; }

        public string Name { get; set; }

        public string OverrideMessage { get; set; }

        public string Description { get; set; }

        public bool ClosedOnly { get; set; }

    }
}