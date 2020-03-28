using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class ScheduleRecurrence
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ConfigId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string Dates { get; set; }

        public string Days { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public bool IsDaySpan { get; set; }

        public bool IsRelative { get; set; }

        public virtual Month? Month { get; set; }

        public virtual RecurrencePattern PatternType { get; set; }

        public virtual RelativeDayType RelativeDayType { get; set; }

        public virtual RelativeMonthlyType RelativeMonthlyType { get; set; }

        public DateTime? WeeklyStartTime { get; set; }

        public DateTime? WeeklyEndTime { get; set; }

    }

    public enum RecurrencePattern
    {
        None,// 0 Recurrence pattern not specified. 
        OneTime,// 1 Schedule occurs only once. 
        Daily,// 2 Schedule repeates daily. 
        Weekly,// 3 Schedule repeats weekly. 
        Monthly,// 4 Schedule repeats monthly. 
        Yearly// 5 Schedule repeats yearly. 
    }

    public enum RelativeDayType
    {
        None,// 0 Recurrence day not specified. 
        Day,// 1 Schedule repeats on the matching day. 
        Monday,// 2 Schedule repeats on the matching Monday. 
        Tuesday,// 3 Schedule repeats on the matching Tuesday. 
        Wednesday,// 4 Schedule repeats on the matching Wednesday. 
        Thursday,// 5 Schedule repeats on the matching Thursday. 
        Friday,// 6 Schedule repeats on the matching Friday. 
        Saturday,// 7 Schedule repeats on the matching Saturday. 
        Sunday// 8 Schedule repeats on the matching Sunday. 
    }

    public enum RelativeMonthlyType
    {
        None,// 0 Recurrence instance not specified. 
        First,// 1 Schedule repeats on the first matching day. 
        Second,// 2 Schedule repeats on the second matching day. 
        Third,// 3 Schedule repeats on the third matching day. 
        Fourth,// 4 Schedule repeats on the fourth matching day. 
        Last// 5 Schedule repeats last matching day. 
    }

    public enum Month
    {
        None,
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public enum Day
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    public enum Date
    {
        None,
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Seventh,
        Eighth,
        Ninth,
        Tenth,
        Eleventh,
        Twelfth,
        Thirteenth,
        Fourteenth,
        Fifteenth,
        Sixteenth,
        Seventeenth,
        Eighteenth,
        Nineteenth,
        Twentieth,
        TwentyFirst,
        TwentySecond,
        TwentyThird,
        TwentyFourth,
        TwentyFifth,
        TwentySixth,
        TwentySeventh,
        TwentyEighth,
        TwentyNinth,
        Thirtieth,
        ThirtyFirst
    }
}
