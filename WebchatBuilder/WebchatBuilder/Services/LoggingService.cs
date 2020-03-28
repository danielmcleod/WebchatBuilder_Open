using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using log4net;
using WebchatBuilder.Controllers;

namespace WebchatBuilder.Services
{
    public class LoggingService
    {
        private static readonly ILog Logger = LogManager.GetLogger("WCB");
        private static LoggingService _cachedLoggingService;

        public static LoggingService GetInstance()
        {
            return _cachedLoggingService ?? (_cachedLoggingService = new LoggingService());
        }

        private LoggingService()
        {

        }

        //public void LogToFile(string logText, string fileName, bool prefixStringWithDateTime = false)
        //{
        //    try
        //    {
        //        if (ChatController.EnableLogging())
        //        {
        //            if (prefixStringWithDateTime)
        //            {
        //                logText = String.Format("{1}-{2}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), logText);
        //            }
        //            if (String.IsNullOrWhiteSpace(fileName))
        //            {
        //                fileName = ChatServices.FolderNameSafeDate();
        //            }
        //            fileName += ".txt";
        //            _counter = _counter > 999 ? 1 : _counter + 1;
        //            logText = String.Format("{0}: {1}", _counter, logText);
        //            Task.Run(() => WriteToLog(logText, fileName));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //LogToFile(String.Format("Logging Error Exception: {0}::StackTrace: {1}{2}", e.Message, e.StackTrace, Environment.NewLine), ChatServices.FolderNameSafeDate(), true);
        //    }
        //}

        //private int _counter = 0;

        //private static readonly object LockObject = new object();

        //public void WriteToLog(string logText, string fileName)
        //{
        //    if (System.Threading.Monitor.TryEnter(LockObject, 1000))
        //    {
        //        try
        //        {
        //            var path = HostingEnvironment.MapPath("~/Logs");
        //            if (path != null)
        //            {
        //                var exists = Directory.Exists(path);
        //                if (!exists)
        //                {
        //                    Directory.CreateDirectory(path);
        //                }

        //                path = Path.Combine(path, fileName);
        //                if (!File.Exists(path))
        //                {
        //                    File.WriteAllText(path, logText);
        //                }
        //                else
        //                {
        //                    File.AppendAllText(path, logText);
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            System.Threading.Monitor.Exit(LockObject);
        //        }
        //    }
        //}

        public void LogException(Exception exception)
        {
            Logger.Error(String.Format("Exception: {0}::StackTrace: {1}", exception.Message, exception.StackTrace));
        }

        public void LogNote(string log)
        {
            if (ChatController.EnableLogging())
            {
                Logger.Info(log);
            }
        }

        public void LogScheduling(string log)
        {
            if (ChatController.EnableLogging())
            {
                Logger.Info(log);
            }
        }

        public void LogMessage(string name, string connectionId, string sessionId, string message, string direction)
        {
            if (ChatController.EnableLogging())
            {
                Logger.Info(String.Format("MSG_SENT::Direction:{0}::SessionId:{1}::ConnectionId:{2}::Name:{3}::Message:{4}", direction, sessionId, connectionId, name, message));
            }
        }

        public void LogWcbResponse(string request, string response)
        {
            if (ChatController.EnableLogging())
            {
                Logger.Info(String.Format("Request: {0} | Response: {1}", request, response));
            }
        }

    }
}