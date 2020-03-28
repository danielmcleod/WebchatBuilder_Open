using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;
using WebchatBuilder.Services;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.Helpers
{
    public class ScheduleManager
    {
        public static Collection<ProfileAvailability> ProfileAvailabilities = new Collection<ProfileAvailability>();

        public static Collection<Profile> ProfilesToReload = new Collection<Profile>();

        private static bool _isReloadingAll;

        private static bool _isReloadingUpdated;

        private static double? _scheduleDateTimeOffset;

        public static DateTime? LastUpdatedUtc { get; set; }

        private static DayProperties _dateProperties = null;

        private static DayProperties DateProperties
        {
            get
            {
                var today = DateTime.UtcNow.AddHours(ScheduleDateTimeOffset).Date;
                if (_dateProperties == null || _dateProperties.Today.Date != today.Date)
                {
                    _dateProperties = GetDayProperties();
                }
                return _dateProperties;
            }
        }

        public static double ScheduleDateTimeOffset
        {
            get
            {
                if (!_scheduleDateTimeOffset.HasValue)
                {
                    var configuredOffset = "";
                    try
                    {
                        configuredOffset = ConfigurationManager.AppSettings["TimeZoneOffset"];
                    }
                    catch (Exception e)
                    {
                        LoggingService.GetInstance().LogException(e);
                    }
                    if (!String.IsNullOrWhiteSpace(configuredOffset))
                    {
                        double timezoneOffset;
                        if (Double.TryParse(configuredOffset, out timezoneOffset))
                        {
                            _scheduleDateTimeOffset = timezoneOffset;
                        }
                    }
                }
                if (!_scheduleDateTimeOffset.HasValue)
                {
                    Double offset = TimeZoneInfo.Local.BaseUtcOffset.TotalHours;
                    if (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.UtcNow))
                    {
                        offset++;
                    }

                    _scheduleDateTimeOffset = offset;
                }

                return _scheduleDateTimeOffset.Value;
            }
        }

        public static void LoadAllSchedules()
        {
            if (_isReloadingAll)
            {
                return;
            }
            RemoveDeletedSchedules();
            _isReloadingAll = true;
            LastUpdatedUtc = DateTime.UtcNow;
            var repository = new Repository();
            var profiles = repository.Profiles.Where(i => i.Schedules.Any()).ToList();
            foreach (var profile in profiles)
            {
                LoadScheduleForProfile(profile);
            }
            _isReloadingAll = false;
        }

        public static void RemoveDeletedSchedules()
        {
            try
            {
                var repository = new Repository();
                if (repository.Schedules.Any(i => i.MarkedForDeletion))
                {
                    var markedForDeletion = repository.Schedules.Where(i => i.MarkedForDeletion);
                    if (markedForDeletion.Any(i => i.Profiles.Any()))
                    {
                        var withProfile = markedForDeletion.Where(i => i.Profiles.Any());
                        foreach (var schedule in withProfile)
                        {
                            schedule.Profiles.Clear();
                        }
                    }
                    repository.Schedules.RemoveRange(markedForDeletion);
                    repository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void UpdateChangedScheduleAvailability()
        {
            if (_isReloadingAll || _isReloadingUpdated)
            {
                return;
            }
            _isReloadingUpdated = true;
            if (ProfilesToReload.Any())
            {
                var profiles = ProfilesToReload.ToList();
                foreach (var profile in profiles)
                {
                    LoadScheduleForProfile(profile);
                    ProfilesToReload.Remove(profile);
                }
            }
            _isReloadingUpdated = false;
        }

        private static readonly object ScheduleLock = new object();

        public static void LoadScheduleForProfile(Profile profile)
        {
            lock (ScheduleLock)
            {
                LoggingService.GetInstance().LogScheduling("Loading Schedules for Profile: " + profile.Name);
                try
                {
                    if (ProfileAvailabilities.Any(i => i.ProfileId == profile.ProfileId))
                    {
                        foreach (var availability in ProfileAvailabilities.Where(i => i.ProfileId == profile.ProfileId).ToList())
                        {
                            ProfileAvailabilities.Remove(availability);
                        }
                    }

                    var dateProperties = DateProperties;

                    var scheduleActiveRanges = new List<ScheduleActiveRange>();
                    foreach (var schedule in profile.Schedules.Where(i => i.IsActive && !i.MarkedForDeletion))
                    {
                        LoggingService.GetInstance().LogScheduling("Processing Schedule: " + schedule.DisplayName);
                        if (schedule.ScheduleRecurrence.StartDate.HasValue && schedule.ScheduleRecurrence.StartDate.Value.Date <= dateProperties.Today.Date)
                        {
                            var allDayStart = dateProperties.Today.Date;
                            var allDayEnd = dateProperties.Today.Date.Add(new TimeSpan(23, 59, 59));
                            if (schedule.ScheduleRecurrence.PatternType == RecurrencePattern.OneTime && schedule.ScheduleRecurrence.EndDate.HasValue && schedule.ScheduleRecurrence.EndDate.Value.Date >= dateProperties.Today.Date)
                            {
                                LoggingService.GetInstance().LogScheduling("Found OneTime Schedule");
                                if (schedule.ScheduleRecurrence.IsAllDay)
                                {
                                    var scheduleActiveRange = new ScheduleActiveRange
                                    {
                                        IsAllDay = true,
                                        IsClosed = schedule.ClosedOnly,
                                        SchedulePriority = SchedulePriority.OneTime,
                                        Message = !String.IsNullOrWhiteSpace(schedule.OverrideMessage) ? schedule.OverrideMessage : schedule.Description,
                                        StartDateTime = allDayStart,
                                        EndDateTime = allDayEnd
                                    };
                                    scheduleActiveRanges.Add(scheduleActiveRange);
                                    LoggingService.GetInstance().LogScheduling("Schedule Added...");
                                }
                                else
                                {
                                    if (schedule.ScheduleRecurrence.StartTime.HasValue && schedule.ScheduleRecurrence.EndTime.HasValue)
                                    {
                                        var startTime = schedule.ScheduleRecurrence.StartTime.Value;
                                        var endTime = schedule.ScheduleRecurrence.EndTime.Value;
                                        var start = dateProperties.Today.Date.Add(new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second));
                                        var end = dateProperties.Today.Date.Add(new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second));

                                        var scheduleActiveRange = new ScheduleActiveRange
                                        {
                                            IsAllDay = false,
                                            IsClosed = schedule.ClosedOnly,
                                            SchedulePriority = SchedulePriority.OneTime,
                                            Message = !String.IsNullOrWhiteSpace(schedule.OverrideMessage) ? schedule.OverrideMessage : schedule.Description,
                                            StartDateTime = start,
                                            EndDateTime = end
                                        };
                                        scheduleActiveRanges.Add(scheduleActiveRange);
                                        LoggingService.GetInstance().LogScheduling("Schedule Added...");
                                    }
                                }
                            }
                            if ((schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Yearly || schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Monthly) && (!schedule.ScheduleRecurrence.EndDate.HasValue || schedule.ScheduleRecurrence.EndDate.Value.Date >= dateProperties.Today.Date))
                            {
                                LoggingService.GetInstance().LogScheduling("Found Yearly or Monthly Schedule");
                                if ((schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Yearly && schedule.ScheduleRecurrence.Month == dateProperties.Month) || schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Monthly)
                                {
                                    var isApplicable = false;

                                    if (schedule.ScheduleRecurrence.IsRelative && schedule.ScheduleRecurrence.RelativeDayType != RelativeDayType.None)
                                    {
                                        if (schedule.ScheduleRecurrence.RelativeDayType == RelativeDayType.Day)
                                        {
                                            switch (schedule.ScheduleRecurrence.RelativeMonthlyType)
                                            {
                                                case RelativeMonthlyType.First:
                                                    isApplicable = dateProperties.Date == 1;
                                                    break;
                                                case RelativeMonthlyType.Second:
                                                    isApplicable = dateProperties.Date == 2;
                                                    break;
                                                case RelativeMonthlyType.Third:
                                                    isApplicable = dateProperties.Date == 3;
                                                    break;
                                                case RelativeMonthlyType.Fourth:
                                                    isApplicable = dateProperties.Date == 4;
                                                    break;
                                                case RelativeMonthlyType.Last:
                                                    isApplicable = dateProperties.LastDayOfMonth;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            if (schedule.ScheduleRecurrence.RelativeDayType == dateProperties.RelativeDay)
                                            {
                                                if (schedule.ScheduleRecurrence.RelativeMonthlyType == dateProperties.RelativeMonthly || (schedule.ScheduleRecurrence.RelativeMonthlyType == RelativeMonthlyType.Last && dateProperties.LastDayOccurenceOfMonth))
                                                {
                                                    isApplicable = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var dates = schedule.ScheduleRecurrence.Dates.Split('|');
                                        if (dates.Any(i => i == dateProperties.Date.ToString()))
                                        {
                                            isApplicable = true;
                                        }
                                    }

                                    if (isApplicable)
                                    {
                                        if (schedule.ScheduleRecurrence.IsAllDay)
                                        {
                                            var scheduleActiveRange = new ScheduleActiveRange
                                            {
                                                IsAllDay = true,
                                                IsClosed = schedule.ClosedOnly,
                                                SchedulePriority = schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Yearly ? SchedulePriority.Yearly : SchedulePriority.Monthly,
                                                Message = !String.IsNullOrWhiteSpace(schedule.OverrideMessage) ? schedule.OverrideMessage : schedule.Description,
                                                StartDateTime = allDayStart,
                                                EndDateTime = allDayEnd
                                            };
                                            scheduleActiveRanges.Add(scheduleActiveRange);
                                            LoggingService.GetInstance().LogScheduling("Schedule Added...");
                                        }
                                        else if (schedule.ScheduleRecurrence.StartTime.HasValue && schedule.ScheduleRecurrence.EndTime.HasValue)
                                        {
                                            var startTime = schedule.ScheduleRecurrence.StartTime.Value;
                                            var endTime = schedule.ScheduleRecurrence.EndTime.Value;
                                            var start = dateProperties.Today.Date.Add(new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second));
                                            var end = startTime > endTime ? allDayEnd : dateProperties.Today.Date.Add(new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second));
                                            var scheduleActiveRange = new ScheduleActiveRange
                                            {
                                                IsAllDay = false,
                                                IsClosed = schedule.ClosedOnly,
                                                SchedulePriority = schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Yearly ? SchedulePriority.Yearly : SchedulePriority.Monthly,
                                                Message = !String.IsNullOrWhiteSpace(schedule.OverrideMessage) ? schedule.OverrideMessage : schedule.Description,
                                                StartDateTime = start,
                                                EndDateTime = end
                                            };
                                            scheduleActiveRanges.Add(scheduleActiveRange);
                                            LoggingService.GetInstance().LogScheduling("Schedule Added...");
                                        }
                                    }

                                }
                            }

                            if (schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Weekly && (!schedule.ScheduleRecurrence.EndDate.HasValue || schedule.ScheduleRecurrence.EndDate.Value.Date >= dateProperties.Today.Date))
                            {
                                LoggingService.GetInstance().LogScheduling("Found Weekly Schedule");

                                var isApplicable = false;
                                var days = schedule.ScheduleRecurrence.Days.Split('|');
                                var isAllDay = schedule.ScheduleRecurrence.IsAllDay;
                                TimeSpan? endTimeOverride = null;
                                if (schedule.ScheduleRecurrence.IsDaySpan && schedule.ScheduleRecurrence.WeeklyStartTime.HasValue && schedule.ScheduleRecurrence.WeeklyEndTime.HasValue)
                                {
                                    var weekStartTime = schedule.ScheduleRecurrence.WeeklyStartTime.Value.TimeOfDay;
                                    var weekEndTime = schedule.ScheduleRecurrence.WeeklyEndTime.Value.TimeOfDay;
                                    var firstDay = days.First();
                                    var lastDay = days.Last();
                                    if (firstDay == lastDay)
                                    {
                                        if (weekEndTime < weekStartTime && firstDay == dateProperties.Day.ToString())
                                        {
                                            endTimeOverride = weekEndTime;
                                        }
                                        isApplicable = true;
                                    }
                                    else
                                    {
                                        int firstInt;
                                        int lastInt;
                                        if (int.TryParse(firstDay, out firstInt) && int.TryParse(lastDay, out lastInt))
                                        {
                                            var range = new List<int>();
                                            if (firstInt < lastInt)
                                            {
                                                var range1 = new int[(lastInt - firstInt) + 1];
                                                var f = 0;
                                                for (var i = firstInt; i <= lastInt; i++)
                                                {
                                                    range1[f] = i;
                                                    f++;
                                                }
                                                range.AddRange(range1);
                                            }
                                            else
                                            {
                                                var range1 = new int[7 - firstInt];
                                                var f = 0;
                                                for (var i = firstInt; i < 7; i++)
                                                {
                                                    range1[f] = i;
                                                    f++;
                                                }
                                                range.AddRange(range1);
                                                var range2 = new int[lastInt];
                                                var l = 0;
                                                for (var i = lastInt; i > 0; i--)
                                                {
                                                    range2[l] = i;
                                                    l++;
                                                }
                                                range.AddRange(range2);
                                            }
                                            if (range.Contains(dateProperties.Day))
                                            {
                                                isApplicable = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (days.Any(i => i == dateProperties.Day.ToString()))
                                    {
                                        isApplicable = true;
                                    }
                                }

                                if (isApplicable)
                                {
                                    DateTime startDateTime;
                                    DateTime endDateTime;

                                    if (!isAllDay && schedule.ScheduleRecurrence.StartTime.HasValue && schedule.ScheduleRecurrence.EndTime.HasValue)
                                    {
                                        var startTime = schedule.ScheduleRecurrence.StartTime.Value;
                                        var endTime = schedule.ScheduleRecurrence.EndTime.Value;
                                        startDateTime = dateProperties.Today.Date.Add(new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second));
                                        endDateTime = startTime > endTime ? allDayEnd : dateProperties.Today.Date.Add(new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second));
                                    }
                                    else
                                    {
                                        startDateTime = allDayStart;
                                        endDateTime = allDayEnd;
                                    }
                                    if (endTimeOverride.HasValue)
                                    {
                                        endDateTime = dateProperties.Today.Date.Add(endTimeOverride.Value);
                                    }

                                    if (startDateTime < endDateTime)
                                    {
                                        var scheduleActiveRange = new ScheduleActiveRange
                                        {
                                            IsAllDay = isAllDay,
                                            IsClosed = schedule.ClosedOnly,
                                            SchedulePriority = SchedulePriority.Weekly,
                                            Message = !String.IsNullOrWhiteSpace(schedule.OverrideMessage) ? schedule.OverrideMessage : schedule.Description,
                                            StartDateTime = startDateTime,
                                            EndDateTime = endDateTime
                                        };
                                        scheduleActiveRanges.Add(scheduleActiveRange);
                                        LoggingService.GetInstance().LogScheduling("Schedule Added...");
                                    }
                                }
                            }
                            if (schedule.ScheduleRecurrence.PatternType == RecurrencePattern.Daily && (!schedule.ScheduleRecurrence.EndDate.HasValue || schedule.ScheduleRecurrence.EndDate.Value.Date >= dateProperties.Today.Date))
                            {
                                LoggingService.GetInstance().LogScheduling("Found Daily Schedule");
                                DateTime startDateTime;
                                DateTime endDateTime;

                                if (!schedule.ScheduleRecurrence.IsAllDay && schedule.ScheduleRecurrence.StartTime.HasValue && schedule.ScheduleRecurrence.EndTime.HasValue)
                                {
                                    var startTime = schedule.ScheduleRecurrence.StartTime.Value;
                                    var endTime = schedule.ScheduleRecurrence.EndTime.Value;
                                    startDateTime = dateProperties.Today.Date.Add(new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second));
                                    endDateTime = startTime > endTime ? allDayEnd : dateProperties.Today.Date.Add(new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second));
                                }
                                else
                                {
                                    startDateTime = allDayStart;
                                    endDateTime = allDayEnd;
                                }

                                var scheduleActiveRange = new ScheduleActiveRange
                                {
                                    IsAllDay = schedule.ScheduleRecurrence.IsAllDay,
                                    IsClosed = schedule.ClosedOnly,
                                    SchedulePriority = SchedulePriority.Daily,
                                    Message = !String.IsNullOrWhiteSpace(schedule.OverrideMessage) ? schedule.OverrideMessage : schedule.Description,
                                    StartDateTime = startDateTime,
                                    EndDateTime = endDateTime
                                };
                                scheduleActiveRanges.Add(scheduleActiveRange);
                                LoggingService.GetInstance().LogScheduling("Schedule Added...");
                            }
                        }
                    }

                    var profileAvailability = new ProfileAvailability
                    {
                        ProfileId = profile.ProfileId,
                        ScheduleActiveRanges = scheduleActiveRanges,
                        LastUpdated = DateTime.UtcNow.AddHours(ScheduleDateTimeOffset).Date
                    };
                    ProfileAvailabilities.Add(profileAvailability);
                    LoggingService.GetInstance().LogScheduling("Finished Loading Schedules for Profile...");
                }
                catch (Exception e)
                {
                    LoggingService.GetInstance().LogException(e);
                }
            }
        }

        public static DayProperties GetDayProperties()
        {
            LoggingService.GetInstance().LogScheduling("Getting Date Properties...");

            var today = DateTime.UtcNow.AddHours(ScheduleDateTimeOffset).Date;
            var day = today.Day;
            var dayOfWeek = today.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            var weeksInMonth = (daysInMonth / 7) + (daysInMonth % 7 != 0 ? 1 : 0);
            var week = (double)day/7;
            var currentWeek = Math.Ceiling(week);
            var weekOfDay = Convert.ToInt32(currentWeek);
            var relativeDay = (Day) ((int)dayOfWeek);

            var dayProperties = new DayProperties
            {
                Today = today,
                Month = (Month)today.Month,
                Date = day,
                Day = (int)dayOfWeek+1,
                RelativeDay = GetRelativeDayType(relativeDay),
                LastDayOfMonth = day == daysInMonth,
                LastDayOccurenceOfMonth = weekOfDay == weeksInMonth,
                RelativeMonthly = GetRelativeMonthlyType(weekOfDay)
            };

            LoggingService.GetInstance().LogScheduling(String.Format("Date Properties - Today:{0} Month:{1} Date:{2} Day:{3} RelativeDay:{4} LastDayOfMonth:{5} LastDayOccurrenceOfMonth:{6} RelativeMonthly:{7}",dayProperties.Today,dayProperties.Month, dayProperties.Date, dayProperties.Day, dayProperties.RelativeDay, dayProperties.LastDayOfMonth, dayProperties.LastDayOccurenceOfMonth, dayProperties.RelativeMonthly));

            return dayProperties;
        }

        public static RelativeMonthlyType GetRelativeMonthlyType(int weekOfDay)
        {
            switch (weekOfDay)
            {
                case 1:
                    return RelativeMonthlyType.First;
                case 2:
                    return RelativeMonthlyType.Second;
                case 3:
                    return RelativeMonthlyType.Third;
                case 4:
                    return RelativeMonthlyType.Fourth;
                case 5:
                    return RelativeMonthlyType.Last;
            }
            return RelativeMonthlyType.None;
        }

        public static RelativeDayType GetRelativeDayType(Day day)
        {
            switch (day)
            {
                case Day.Monday:
                    return RelativeDayType.Monday;
                case Day.Tuesday:
                    return RelativeDayType.Tuesday;
                case Day.Wednesday:
                    return RelativeDayType.Wednesday;
                case Day.Thursday:
                    return RelativeDayType.Thursday;
                case Day.Friday:
                    return RelativeDayType.Friday;
                case Day.Saturday:
                    return RelativeDayType.Saturday;
                case Day.Sunday:
                    return RelativeDayType.Sunday;
            }
            return RelativeDayType.None;
        }

        public class DayProperties
        {
            public DateTime Today { get; set; }

            public virtual Month Month { get; set; }

            public int Day { get; set; }

            public int Date { get; set; }

            public virtual RelativeDayType RelativeDay { get; set; }

            public virtual RelativeMonthlyType RelativeMonthly { get; set; }

            public bool LastDayOfMonth { get; set; }

            public bool LastDayOccurenceOfMonth { get; set; }
        }

        public class ProfileAvailability
        {
            public int ProfileId { get; set; }

            public virtual ICollection<ScheduleActiveRange> ScheduleActiveRanges { get; set; }

            public DateTime LastUpdated { get; set; }
        }

        public class ScheduleActiveRange
        {
            public DateTime StartDateTime { get; set; }

            public DateTime EndDateTime { get; set; }

            public bool IsAllDay { get; set; }

            public bool IsClosed { get; set; }

            public virtual SchedulePriority SchedulePriority { get; set; } //should we just use pattern?

            public string Message { get; set; }
        }

        public enum SchedulePriority
        {
            Unplanned,
            OneTime,
            Yearly,
            Monthly,
            Weekly,
            Daily           
        }



    }
}