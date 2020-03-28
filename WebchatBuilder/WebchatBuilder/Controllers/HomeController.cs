using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebchatBuilder.DataModels;
using WebchatBuilder.Helpers;
using WebchatBuilder.Services;
using WebchatBuilder.ViewModels;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.Controllers
{
    [AuthorizeIpAddress]
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly Repository _repository = new Repository();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            var company = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "companyname");
            var logo = _repository.Settings.FirstOrDefault(s => s.Key.ToLower() == "logopath");

            if (company == null)
            {
                company = new Setting
                {
                    Key = "CompanyName",
                    Value = "CompanyName"
                };
                _repository.Settings.Add(company);
                _repository.SaveChanges();
            }
            if (logo == null)
            {
                logo = new Setting
                {
                    Key = "LogoPath",
                    Value = ""
                };
                _repository.Settings.Add(logo);
                _repository.SaveChanges();
            }
            var model = new LandingViewModel
            {
                CompanyName = company.Value,
                CompanyLogoPath = logo.Value
            };
            return View(model);
        }
        [Authorize(Roles = "DashboardAdmin")]
        public ActionResult Dashboard()
        {
            var queuedChats = ChatServices.WebChats.Where(c => c.DateCreated.HasValue && !c.DateAnswered.HasValue).Select(i => i.ChatId);
            var model = new DashboardViewModel
            {
                AbandonedChats = _repository.Chats.Count(c => c.DateCreated.HasValue && !c.DateAnswered.HasValue && !queuedChats.Contains(c.ChatId)),
                ActiveChats = ChatServices.WebChats.Count(c => c.DateAnswered.HasValue && !c.DateEnded.HasValue),
                QueuedChats = queuedChats.Count(),
                TotalChats = _repository.Chats.Count()
            };
            return View(model);
        }

        [Authorize(Roles = "DashboardAdmin")]
        public ActionResult RefreshDashboard(DateTime? startDate = null, DateTime? endDate = null)
        {
            var queuedChats = ChatServices.WebChats.Where(c => c.DateCreated.HasValue && !c.DateAnswered.HasValue).Select(i => i.ChatId);
            var profiles = _repository.Profiles.Select(i => i.Name).ToList();
            var chats = _repository.Chats.Where(i => true);
            if (startDate.HasValue && endDate.HasValue)
            {
                chats = _repository.Chats.Where(i => i.DateCreated >= startDate.Value && i.DateCreated <= endDate.Value);
            }
            var model = new DashboardViewModel
            {
                AbandonedChats = chats.Count(c => c.DateCreated.HasValue && !c.DateAnswered.HasValue && !queuedChats.Contains(c.ChatId)),
                ActiveChats = ChatServices.WebChats.Count(c => c.DateAnswered.HasValue && !c.DateEnded.HasValue),
                QueuedChats = queuedChats.Count(),
                TotalChats = chats.Count(),
                Profiles = profiles
            };
            return PartialView("_DashboardCards", model);
        }

        [Authorize(Roles = "DashboardAdmin")]
        public JsonResult GetWorkgroupStats(string profileName, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var profile = _repository.Profiles.FirstOrDefault(p => p.Name == profileName);
                if (profile != null)
                {
                    //var workgroup = profile.Workgroup.DisplayName;
                    var queueChats = ChatServices.WebChats.Where(c => c.ProfileName == profileName).ToList();
                    var queuedChats = queueChats.Where(c => c.DateCreated.HasValue && !c.DateAnswered.HasValue).Select(i => i.ChatId).ToList();
                    var active = queueChats.Count(c => c.DateAnswered.HasValue && !c.DateEnded.HasValue);
                    var queued = queuedChats.Count();
                    var queueHistory = _repository.Chats.Where(i => i.Profile == profileName);
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        queueHistory = queueHistory.Where(i => i.DateCreated >= startDate.Value && i.DateCreated <= endDate.Value);
                    }
                    var total = queueHistory.Count();
                    var abandoned = queueHistory.Count(c => c.DateCreated.HasValue && !c.DateAnswered.HasValue && !queuedChats.Contains(c.ChatId));
                    return Json(new { success = true, active, queued, total, abandoned }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Supervisor")]
        public ActionResult WebChats()
        {
            return View();
        }

        public JsonResult GetChatDetails(long chatId)
        {
            try
            {
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.ChatId == chatId);
                if (webChat != null)
                {
                    var messages = webChat.Messages;
                    return Json(new { success = true, chatId = webChat.ChatId, messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReportIssue()
        {
            try
            {
                if (ChatServices.RestartService())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DisconnectChat(long chatId)
        {
            try
            {
                var webChat = ChatServices.WebChats.FirstOrDefault(c => c.ChatId == chatId);
                if (webChat != null)
                {
                    ChatServices.ProcessDisconnect(webChat.ConnectionId, false, true);
                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false });
        }

        //public JsonResult ServiceLog()
        //{
        //    return Json(new { log = ChatServices.ServiceLog() }, JsonRequestBehavior.AllowGet);
        //}

        [Authorize(Roles = "Supervisor")]
        public ActionResult VisitorMessages()
        {
            var model = new VisitorMessagesViewModel
            {
                VisitorMessages = _repository.VisitorMessages.Where(i => !i.IsProcessed).ToList(),
                ProcessedMessages = _repository.VisitorMessages.Where(i => i.IsProcessed).ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public JsonResult DeleteVisitorMessage(long id)
        {
            var message = "Error Deleting Message.";
            try
            {
                var vm = _repository.VisitorMessages.FirstOrDefault(i => i.VisitorMessageId == id);
                if (vm != null)
                {
                    _repository.VisitorMessages.Remove(vm);
                    _repository.SaveChanges();
                    return Json(new { success = true });
                }
                message = "Message not found.";
            }
            catch (Exception)
            {
            }

            return Json(new { success = false, message });
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost]
        public JsonResult ResendVisitorMessage(long id)
        {
            var message = "Error Resending Message.";
            try
            {
                var vm = _repository.VisitorMessages.FirstOrDefault(i => i.VisitorMessageId == id);
                if (vm != null)
                {
                    vm.IsProcessed = false;
                    _repository.SaveChanges();
                    return Json(new { success = true });
                }
                message = "Message not found.";
            }
            catch (Exception)
            {
            }

            return Json(new { success = false, message });
        }

        [Authorize(Roles = "UserAdmin")]
        public ActionResult Users()
        {
            var users = UserManager.Users.Where(i => !i.IsDeleted);
            return View(users.ToList());
        }

        public ActionResult GetUserInfo()
        {
            try
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var imgPath = String.IsNullOrEmpty(user.DisplayImage) ? "/Content/Images/WcbUser.png" : user.DisplayImage;
                var model = new UserViewModel
                {
                    DisplayName = user.DisplayName,
                    Title = user.Title,
                    ImgPath = imgPath
                };
                return PartialView("_UserInfo", model);
            }
            catch (Exception)
            {
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AboutWcb()
        {
            return PartialView("_AboutWcb");
        }

        public JsonResult GetChatStats()
        {
            var success = false;
            try
            {
                if (_repository.Chats.Any())
                {
                    var thisYear = DateTime.Now.Year;
                    var thisMonth = DateTime.Now.Month;
                    var currentHour = DateTime.Now.AddHours(-1);
                    var currentDay = DateTime.Today;
                    var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    var currentMonth = new DateTime(thisYear, thisMonth, 1);
                    var currentYear = new DateTime(thisYear, 1, 1);
                    var hour = _repository.Chats.Count(c => c.DateAnswered > currentHour);
                    var day = _repository.Chats.Count(c => c.DateAnswered > currentDay);
                    var week = _repository.Chats.Count(c => c.DateAnswered > sunday);
                    var month = _repository.Chats.Count(c => c.DateAnswered > currentMonth);
                    var year = _repository.Chats.Count(c => c.DateAnswered > currentYear);
                    var total = _repository.Chats.Count(c => c.DateAnswered.HasValue);
                    success = true;
                    return Json(new{success,hour,day,week,month,year,total}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
            }
            return Json(new{success}, JsonRequestBehavior.AllowGet);
        }

        //GetStatuses
        public JsonResult GetStatuses()
        {
            var success = false;
            try
            {
                var cicStatus = ChatServices.GetConfig();
                var serviceStatus = ChatServices.IsLicensed();
                var wcbStatus = cicStatus && serviceStatus;
                var cicName = ChatServices.GetCicServerName;
                var wcbName = GetServerIpAddress();
                var port = ConfigurationManager.AppSettings["WcbServicePort"];
                if (String.IsNullOrWhiteSpace(port))
                {
                    port = "8088";
                }
                if (!cicStatus)
                {
                    ChatServices.AddAlert("Server Error", "Error connecting to CIC server.", 0001);
                }
                if (!serviceStatus)
                {
                    ChatServices.AddAlert("Server Error", "Error connecting to WCB Service.", 0002);
                }
                var serviceName = "http://localhost:" + port;
                success = true;
                return Json(new {success, cicStatus, cicName, serviceStatus, serviceName, wcbStatus, wcbName}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(new { success }, JsonRequestBehavior.AllowGet);
        }

        public string GetServerIpAddress()
        {
            try
            {
                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = ipHostInfo.AddressList.Any(i => i.AddressFamily == AddressFamily.InterNetwork) ? ipHostInfo.AddressList.FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork) : ipHostInfo.AddressList.FirstOrDefault();
                return ipAddress.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}