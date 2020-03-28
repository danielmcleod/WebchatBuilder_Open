using System;
using System.Web.Http;
using WebChatBuilderService.Helpers;
using WebChatBuilderService.Icelib;
using WebChatBuilderService.Icelib.Availability;
using WebChatBuilderService.Icelib.Configurations;
using WebChatBuilderService.Icelib.Interactions;
using WebChatBuilderService.Icelib.People;
using WebChatBuilderService.Services;

namespace WebChatBuilderService.Controllers
{
    [AuthorizeIpAddress]
    public class ServicesController : ApiController
    {
        private Logging _logging;
        private DateTime? _lastRestart = null;

        [HttpGet]
        public bool IsLicensed()
        {
            return LicenseService.ValidateLicense();
        }

        [HttpGet]
        public string GetCicServer()
        {
            return IcelibInitializer.ActiveServer;
        }

        [HttpGet]
        public int AgentsAvailable(int profileId)
        {
            return AgentAvailability.GetAgentAvailabilityByProfile(profileId);
        }

        [HttpGet]
        public DateTime? GetLastScheduleUpdate()
        {
            return ScheduleConfigurations.LastUpdated;
        }

        //[HttpGet]
        //public string GetLog()
        //{
        //    var def = "...";
        //    try
        //    {
        //        var running = Logging.RunningLog;
        //        var log = !String.IsNullOrWhiteSpace(running) ? running : def;
        //        return log;
        //    }
        //    catch (Exception e)
        //    {
        //        _logging.TraceException(e,e.Message);
        //    }
        //    return def;
        //}

        [HttpGet]
        public bool UpdateRefreshWatch()
        {
            WorkgroupPeople.RefreshWatch = true;
            WorkgroupInteractions.RefreshWatch = true;
            return true;
        }

        [HttpGet]
        public bool RestartService()
        {
            try
            {
                var now = DateTime.Now;
                if (_lastRestart.HasValue && _lastRestart.Value.AddMinutes(5) > now)
                {
                    return false;
                }

                _lastRestart = now;
                if (WcbService.IcelibInitializer != null)
                {
                    WcbService.IcelibInitializer.Stop();
                    WcbService.IcelibInitializer.Start();
                }
                return true;
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }
    }
}
