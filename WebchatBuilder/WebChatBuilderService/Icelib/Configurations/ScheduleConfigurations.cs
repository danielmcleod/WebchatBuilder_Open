using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ININ.IceLib.Configuration;
using ININ.IceLib.Connection;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebChatBuilderService.Icelib.Configurations
{
    public class ScheduleConfigurations
    {
        private ConfigurationManager _configurationManager;
        private Logging _logging;

        private ScheduleConfigurationList _scheduleConfigurationList;

        private ICollection<ScheduleConfiguration> _schedules = new Collection<ScheduleConfiguration>();

        public static DateTime? LastUpdated { get; set; }

        protected Repository Repository
        {
            get { return new Repository(); }
        }

        public void Load(Session session)
        {
            _configurationManager = ConfigurationManager.GetInstance(session);
            _logging = Logging.GetInstance();
            StartWatchingSchedules();
        }

        public void Unload()
        {
            try
            {
                if (_scheduleConfigurationList.IsWatching)
                {
                    _scheduleConfigurationList.StopWatching();
                }
            }
            catch (Exception)
            {
            }
            _scheduleConfigurationList = null;
            _configurationManager = null;
        }

        public void RefreshWatches()
        {
            if (_scheduleConfigurationList.IsWatching)
            {
                _scheduleConfigurationList.RefreshWatchAsync(Schedules_StartOrRefreshWatchingCompleted, null);
            }
        }

        private void StartWatchingSchedules()
        {
            try
            {
                if (_configurationManager != null && _configurationManager.Session != null && _configurationManager.Session.ConnectionState == ConnectionState.Up)
                {
                    _scheduleConfigurationList = new ScheduleConfigurationList(_configurationManager);
                    var querySettings = _scheduleConfigurationList.CreateQuerySettings();
                    querySettings.SetRightsFilterToAdmin();
                    querySettings.SetPropertiesToRetrieveToAll();
                    querySettings.SetResultCountLimit(QueryResultLimit.Unlimited);
                    //querySettings.SetFilterDefinition(ScheduleConfiguration.Property.Id, name);

                    //var recurranceQuery = _scheduleConfigurationList.CreateRecurrenceQuerySettings();
                    var recurranceQuery = new QuerySettings<ScheduleRecurrenceConfiguration, ScheduleRecurrenceConfiguration.Property>();
                    var queryChildren = new ScheduleQueryChildrenSettings();
                    recurranceQuery.SetPropertiesToRetrieveToAll();
                    queryChildren.ScheduleRecurrence = recurranceQuery;
                    querySettings.SetChildQuerySettings(queryChildren);

                    using (var repository = Repository)
                    {
                        if (!repository.Schedules.Any())
                        {
                            _scheduleConfigurationList.StartCaching(querySettings);
                            var schedules = _scheduleConfigurationList.GetConfigurationList();
                            _scheduleConfigurationList.StopCaching();
                            if (schedules.Any())
                            {
                                foreach (var scheduleConfiguration in schedules)
                                {
                                    CreateSchedule(scheduleConfiguration);
                                }
                                _schedules = schedules;
                            }
                        }
                    }

                    _scheduleConfigurationList.ConfigurationObjectsRemoved += Schedules_ConfigurationObjectsRemoved;

                    _scheduleConfigurationList.StartWatchingAsync(querySettings, Schedules_StartOrRefreshWatchingCompleted, null);

                    LastUpdated = DateTime.UtcNow;
                }
            }
            catch (Exception e)
            {
                _logging.TraceException(e, "ScheduleConfigurations Error");
            }
        }

        private void Schedules_StartOrRefreshWatchingCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var scheduleConfigurationList = sender as ScheduleConfigurationList;
            ProcessScheduleConfigurations(scheduleConfigurationList);
        }

        private static readonly object ProcessingLock = new object();

        private void ProcessScheduleConfigurations(ScheduleConfigurationList scheduleConfigurationList)
        {
            lock (ProcessingLock)
            try
            {
                if (scheduleConfigurationList != null)
                {
                    var schedules = scheduleConfigurationList.GetConfigurationList();
                    if (!_schedules.Any() && schedules.Any())
                    {
                        foreach (var scheduleConfiguration in schedules.Distinct())
                        {
                            CreateSchedule(scheduleConfiguration);
                        }
                        _schedules = schedules;
                        LastUpdated = DateTime.UtcNow;
                    }
                    else if (SchedulesChanged(schedules))
                    {
                        _logging.LogNote("Schedules Changed");
                        foreach (var scheduleConfiguration in schedules)
                        {
                            CreateSchedule(scheduleConfiguration);
                        }
                        _schedules = schedules;
                        LastUpdated = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.TraceException(exception, "ScheduleConfigurations Error");
            }
        }

        private void CreateSchedule(ScheduleConfiguration scheduleConfiguration)
        {
            try
            {
                using (var repository = Repository)
                {
                    var existingSchedule = repository.Schedules.FirstOrDefault(s => s.ConfigId == scheduleConfiguration.ConfigurationId.Id);
                    var scheduleRecurrence = scheduleConfiguration.ScheduleRecurrences.Value.FirstOrDefault();
                    var defaultDate = new DateTime(2000, 01, 01);
                    if (existingSchedule == null)
                    {
                        if (scheduleRecurrence != null)
                        {
                            var schedRecurrence = GetScheduleRecurrence(scheduleRecurrence);
                            repository.ScheduleRecurrences.Add(schedRecurrence);
                            var dateLastModified = scheduleConfiguration.StandardProperties.DateLastModified != null && scheduleConfiguration.StandardProperties.DateLastModified.Value > defaultDate ? scheduleConfiguration.StandardProperties.DateLastModified.Value : scheduleConfiguration.StandardProperties.DateCreated != null && scheduleConfiguration.StandardProperties.DateCreated.Value > defaultDate ? scheduleConfiguration.StandardProperties.DateCreated.Value : defaultDate;
                            //_logging.TraceMessage(3, "Date Last Modified: " + dateLastModified.ToShortDateString());
                            var hasKeywords = scheduleConfiguration.Keywords != null && scheduleConfiguration.Keywords.Value != null;
                            var keywords = hasKeywords ? String.Join("|", scheduleConfiguration.Keywords.Value) : String.Empty;
                            var schedule = new Schedule
                            {
                                ConfigId = scheduleConfiguration.ConfigurationId.Id,
                                DisplayName = scheduleConfiguration.ConfigurationId.DisplayName,
                                Description = scheduleConfiguration.Description.Value,
                                IsActive = scheduleConfiguration.IsActive.Value,
                                IsAssignable = hasKeywords && scheduleConfiguration.Keywords.Value.Any(i => i.ToLower() == "wcb"),
                                MarkedForDeletion = false,
                                Profiles = new List<Profile>(),
                                ScheduleRecurrence = schedRecurrence,
                                DateLastModified = dateLastModified,
                                Keywords = keywords,
                                ClosedOnly = hasKeywords && scheduleConfiguration.Keywords.Value.Any(i => i.ToLower() == "closed")
                            };
                            repository.Schedules.Add(schedule);
                            repository.SaveChanges();
                            _logging.LogNote("Schedule Created: " + schedule.DisplayName);
                        }
                    }
                    else
                    {
                        existingSchedule.DisplayName = scheduleConfiguration.ConfigurationId.DisplayName;
                        existingSchedule.Description = scheduleConfiguration.Description.Value;
                        existingSchedule.IsActive = scheduleConfiguration.IsActive.Value;
                        existingSchedule.DateLastModified = scheduleConfiguration.StandardProperties.DateLastModified != null && scheduleConfiguration.StandardProperties.DateLastModified.Value > defaultDate ? scheduleConfiguration.StandardProperties.DateLastModified.Value : scheduleConfiguration.StandardProperties.DateCreated != null && scheduleConfiguration.StandardProperties.DateCreated.Value > defaultDate ? scheduleConfiguration.StandardProperties.DateCreated.Value : defaultDate;
                        if (scheduleRecurrence != null)
                        {
                            var schedRecurrence = GetScheduleRecurrence(scheduleRecurrence);
                            if (existingSchedule.ScheduleRecurrence != null && !existingSchedule.ScheduleRecurrence.Equals(schedRecurrence))
                            {
                                //_logging.TraceMessage(3, String.Format("Schedule {0} Changed.", schedRecurrence.DisplayName));
                                var existingRecurrence = existingSchedule.ScheduleRecurrence;
                                existingRecurrence.ConfigId = schedRecurrence.ConfigId;
                                existingRecurrence.DisplayName = schedRecurrence.DisplayName;
                                existingRecurrence.IsAllDay = schedRecurrence.IsAllDay;
                                existingRecurrence.IsDaySpan = schedRecurrence.IsDaySpan;
                                existingRecurrence.IsRelative = schedRecurrence.IsRelative;
                                existingRecurrence.Dates = schedRecurrence.Dates;
                                existingRecurrence.Days = schedRecurrence.Days;
                                existingRecurrence.Month = schedRecurrence.Month;
                                existingRecurrence.PatternType = schedRecurrence.PatternType;
                                existingRecurrence.RelativeDayType = schedRecurrence.RelativeDayType;
                                existingRecurrence.RelativeMonthlyType = schedRecurrence.RelativeMonthlyType;
                                existingRecurrence.StartDate = schedRecurrence.StartDate;
                                existingRecurrence.EndDate = schedRecurrence.EndDate;
                                existingRecurrence.StartTime = schedRecurrence.StartTime;
                                existingRecurrence.EndTime = schedRecurrence.EndTime;
                                existingRecurrence.WeeklyStartTime = schedRecurrence.WeeklyStartTime;
                                existingRecurrence.WeeklyEndTime = schedRecurrence.WeeklyEndTime;
                            }
                        }
                        _logging.LogNote("Schedule Created: " + existingSchedule.DisplayName);

                        repository.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                _logging.TraceException(e, "ScheduleConfigurations Error");
            }
        }

        private ScheduleRecurrence GetScheduleRecurrence(ScheduleRecurrenceConfiguration scheduleRecurrence)
        {
            var defaultDate = new DateTime(1753, 01, 01);
            var datesList = scheduleRecurrence.Dates.Value.ToList();
            var dates = String.Join("|", datesList);
            //var daysList = scheduleRecurrence.Days.Value.Select(i => (Day)i).ToList();
            var daysList = scheduleRecurrence.Days.Value.ToList();
            var days = String.Join("|", daysList);
            var month = (Month?)scheduleRecurrence.Month.Value;
            var pattern = (RecurrencePattern)scheduleRecurrence.PatternType.Value;
            var relativeDay = (RelativeDayType)scheduleRecurrence.RelativeDay.Value;
            var relativeMonthly = (RelativeMonthlyType)scheduleRecurrence.RelativeMonthlyInstance.Value;
            var startDate = scheduleRecurrence.StartDate != null && scheduleRecurrence.StartDate.Value != null && scheduleRecurrence.StartDate.Value.DateValue > defaultDate ? scheduleRecurrence.StartDate.Value.DateValue.Date as DateTime? : null;
            var endDate = scheduleRecurrence.EndDate != null && scheduleRecurrence.EndDate.Value != null && scheduleRecurrence.EndDate.Value.DateValue > defaultDate ? scheduleRecurrence.EndDate.Value.DateValue.Date as DateTime? : null;
            var startTime = scheduleRecurrence.StartTime != null && scheduleRecurrence.StartTime.Value != null && scheduleRecurrence.StartTime.Value.TimeValue > defaultDate ? scheduleRecurrence.StartTime.Value.TimeValue as DateTime? : null;
            var endTime = scheduleRecurrence.EndTime != null && scheduleRecurrence.EndTime.Value != null && scheduleRecurrence.EndTime.Value.TimeValue > defaultDate ? scheduleRecurrence.EndTime.Value.TimeValue as DateTime? : null;
            var weeklyStartTime = scheduleRecurrence.WeeklyStartTime != null && scheduleRecurrence.WeeklyStartTime.Value != null && scheduleRecurrence.WeeklyStartTime.Value.TimeValue > defaultDate ? scheduleRecurrence.WeeklyStartTime.Value.TimeValue as DateTime? : null;
            var weeklyEndTime = scheduleRecurrence.WeeklyEndTime != null && scheduleRecurrence.WeeklyEndTime.Value != null && scheduleRecurrence.WeeklyEndTime.Value.TimeValue > defaultDate ? scheduleRecurrence.WeeklyEndTime.Value.TimeValue as DateTime? : null;

            var schedRecurrence = new ScheduleRecurrence
            {
                ConfigId = scheduleRecurrence.ConfigurationId.Id,
                DisplayName = scheduleRecurrence.ConfigurationId.DisplayName,
                IsAllDay = scheduleRecurrence.IsAllDay.Value,
                IsDaySpan = scheduleRecurrence.IsDaySpan.Value,
                IsRelative = scheduleRecurrence.IsRelative.Value,
                Dates = dates,
                Days = days,
                Month = month,
                PatternType = pattern,
                RelativeDayType = relativeDay,
                RelativeMonthlyType = relativeMonthly,
                StartDate = startDate,
                StartTime = startTime,
                EndDate = endDate,
                EndTime = endTime,
                WeeklyStartTime = weeklyStartTime,
                WeeklyEndTime = weeklyEndTime
            };
            return schedRecurrence;
        }

        private bool SchedulesChanged(ICollection<ScheduleConfiguration> schedules)
        {
            if (schedules.Count != _schedules.Count)
            {
                return true;
            }

            var scheduleIds = schedules.Select(s => s.ConfigurationId.Id);

            if (scheduleIds.Any(i => !_schedules.Select(s => s.ConfigurationId.Id).Contains(i)))
            {
                return true;
            }
            foreach (var scheduleConfiguration in schedules)
            {
                var sched = _schedules.FirstOrDefault(s => s.ConfigurationId.Id == scheduleConfiguration.ConfigurationId.Id);
                if (sched != null && sched.StandardProperties.DateLastModified.Value < scheduleConfiguration.StandardProperties.DateLastModified.Value)
                {
                    return true;
                }
            }

            return false;
        }

        private void Schedules_ConfigurationObjectsRemoved(object sender, ConfigurationWatchEventArgs<ScheduleConfiguration> e)
        {
            try
            {
                var removeSchedules = e.ObjectsAffected;
                using (var repository = Repository)
                {
                    foreach (var schedule in removeSchedules.Select(scheduleConfiguration => repository.Schedules.FirstOrDefault(s => s.ConfigId == scheduleConfiguration.ConfigurationId.Id)).Where(schedule => schedule != null))
                    {
                        _logging.TraceMessage(3,"Removing Schedule: " + schedule.DisplayName);
                        if (schedule.ScheduleRecurrence != null)
                        {
                            repository.ScheduleRecurrences.Remove(schedule.ScheduleRecurrence);
                        }
                        if (schedule.IsUsed)
                        {
                            schedule.MarkedForDeletion = true;
                        }
                        else
                        {
                            repository.Schedules.Remove(schedule);
                        }
                        repository.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.TraceException(exception, "ScheduleConfigurations Error");
            }
        }

    }

}
