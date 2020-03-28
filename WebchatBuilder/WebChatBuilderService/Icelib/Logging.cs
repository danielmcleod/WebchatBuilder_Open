using System;
using System.Diagnostics;
using log4net;

namespace WebChatBuilderService.Icelib
{
    public class Logging
    {
        private static readonly ILog Logger = LogManager.GetLogger("WCB");
        private static Logging _logging;

        public static Logging GetInstance()
        {
            return _logging ?? (_logging = new Logging());
        }

        private int _eventIdCounter = 100;
        const string EventSource = "WebChatBuilder";
        const string EventLogType = "Application";
        public void ApplicationEventLog(string eventText, int? eventId, EventLogEntryType eventLogEntryType = EventLogEntryType.Information)
        {
            int eventLogId = eventId ?? _eventIdCounter++;
            if (!EventLog.SourceExists(EventSource))
            {
                EventLog.CreateEventSource(EventSource, EventLogType);
            }
            EventLog.WriteEntry(EventSource, eventText, eventLogEntryType, eventLogId);
        }

        public void TraceMessage(int level, string message)
        {
            switch (level)
            {
                //0 Always
                case 0: 
                    ApplicationEventLog(message,0,EventLogEntryType.Information);
                    break;
                //1 Error
                case 1:
                    ApplicationEventLog(message, 1, EventLogEntryType.Error);
                    break;
                //2 Warning
                case 2:
                    ApplicationEventLog(message, 2, EventLogEntryType.Warning);
                    break;
                //3 Note
                case 3:
                    ApplicationEventLog(message, 3, EventLogEntryType.Information);
                    break;
            }
        }

        public void TraceException(Exception exception, string message)
        {
            ApplicationEventLog(message + " " + exception.ToString(), 4, EventLogEntryType.Error);
        }

        public void LogNote(string log)
        {
            Logger.Info(log);
        }

        public void LogException(Exception ex)
        {
            var log = ex.Message + Environment.NewLine;
            log += ex.StackTrace;
            Logger.Error(log);
        }
    }
}
