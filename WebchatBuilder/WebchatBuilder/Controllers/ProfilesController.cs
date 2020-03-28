using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebchatBuilder.Helpers;
using WebchatBuilder.Services;
using WebchatBuilder.ViewModels;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.Controllers
{
    [AuthorizeIpAddress]
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly Repository _repository = new Repository();
        // GET: Profiles
        [HttpGet]
        public ActionResult GetProfiles()
        {
            if (!_repository.Profiles.Any())
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            var model = _repository.Profiles.Select(p => new ProfileViewModel
            {
                ProfileId = p.ProfileId,
                ProfileName = p.Name,
                Description = p.Description,
                LastUpdatedBy = p.LastUpdatedBy.DisplayName,
                LastUpdatedOn = p.LastUpdatedOn.ToString(),
                Template = p.Template.Title,
                Workgroup = p.Workgroup.DisplayName,
                Widget = p.Widget != null ? p.Widget.Name : "",
                Schedules = p.Schedules.Select(s => s.DisplayName).ToList(),
                Skills = p.Skills.Select(s => s.DisplayName).ToList(),
                HasError = p.Workgroup.MarkedForDeletion || p.Skills.Any(s => s.MarkedForDeletion),
                ErrorMessage = p.Workgroup.MarkedForDeletion && p.Skills.Any(s => s.MarkedForDeletion) ? "This profile contains Workgroups and Skills that no longer exist in CIC." : p.Workgroup.MarkedForDeletion ? "This profile contains Workgroups that no longer exist in CIC." : p.Skills.Any(s => s.MarkedForDeletion) ? "This profile contains skills that no longer exist in CIC." : ""
            }).OrderBy(p => p.ProfileName).ToList();
            return PartialView("_Profiles", model);
        }

        void UpdateSchedules(Profile profile)
        {
            Task.Run(() => ScheduleManager.LoadScheduleForProfile(profile));
        }

        [Authorize (Roles = "ProfileAdmin")]
        [HttpGet]
        public ActionResult CreateProfile()
        {
            var message = "";
            try
            {
                if (!_repository.Workgroups.Any(w => w.IsAssignable))
                {
                    throw new Exception("No Assignable Workgroups found.");
                }
                if (!_repository.Templates.Any())
                {
                    throw new Exception("No Templates found. Please create one to continue.");
                }
                //Create a default widget and Template
                var profileName = "";
                const string defaultName = "Profile";
                var cnt = 1;
                var distinct = false;
                while (!distinct)
                {
                    profileName = defaultName + cnt;
                    var existing = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == profileName.ToLower());
                    if (existing != null)
                    {
                        cnt++;
                    }
                    else
                    {
                        distinct = true;
                    }
                }
                var user = _repository.Users.Find(User.Identity.GetUserId());                
                var template = _repository.Templates.FirstOrDefault();
                var workgroup = _repository.Workgroups.FirstOrDefault(i => i.IsAssignable);
                //var widget = _repository.Widgets.FirstOrDefault();
                var profile = new Profile
                {
                    Name = profileName,
                    CreatedBy = user,
                    CreatedOn = DateTime.Now,
                    LastUpdatedBy = user,
                    LastUpdatedOn = DateTime.Now,
                    Workgroup = workgroup,
                    Template = template,
                    Widget = null,
                    IncludeUserDataAsAttributes = false,
                    IncludeUserDataAsCustomInfo = false,
                    AllowAttachments = true
                };
                _repository.Profiles.Add(profile);
                _repository.SaveChanges();
                var model = GetCreateProfileViewModel(profile);
                ChatServices.UpdateRefreshWatch();
                return PartialView("_CreateProfile", model); 
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "ProfileAdmin")]
        [HttpGet]
        public ActionResult EditProfile(int profileId)
        {
            try
            {
                var profile = _repository.Profiles.Find(profileId);
                if (profile != null)
                {
                    var model = GetCreateProfileViewModel(profile);
                    return PartialView("_CreateProfile", model);
                }
            }
            catch (Exception)
            {

            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        private CreateProfileViewModel GetCreateProfileViewModel(Profile profile)
        {
            var workgroupList = _repository.Workgroups.Where(w => w.IsAssignable).Select(w => new SelectListItem
            {
                Text = w.DisplayName,
                Value = w.WorkgroupId.ToString(),
                Selected = w.WorkgroupId == profile.Workgroup.WorkgroupId
            }).ToList();
            var templateList = _repository.Templates.Select(t => new SelectListItem
            {
                Text = t.Title,
                Value = t.TemplateId.ToString(),
                Selected = t.TemplateId == profile.Template.TemplateId
            }).ToList();
            var widgetList = !_repository.Widgets.Any() ? new List<SelectListItem>() : _repository.Widgets.Select(w => new SelectListItem
                {
                    Text = w.Name,
                    Value = w.WidgetId.ToString()
                    //Selected = profile.Widget != null && w.WidgetId == profile.Widget.WidgetId
                }).ToList();
            widgetList.Add(new SelectListItem
            {
                Text = "None",
                Value = "-1"
                //Selected = profile.Widget == null
            });

            var skillsList = !_repository.Skills.Any(s => s.IsAssignable) ? new List<Skill>() : _repository.Skills.Where(s => s.IsAssignable).ToList();
            var skills = new List<int>();
            if (profile.Skills != null && profile.Skills.Any())
            {
                skills = profile.Skills.Select(s => s.SkillId).ToList();
            }
            var scheduleList = !_repository.Schedules.Any(s => s.IsAssignable) ? new List<Schedule>() : _repository.Schedules.Where(s => s.IsAssignable).ToList();
            var schedules = new List<int>();
            if (profile.Schedules != null && profile.Schedules.Any())
            {
                schedules = profile.Schedules.Select(s => s.Id).ToList();
            }
            var model = new CreateProfileViewModel
            {
                ProfileId = profile.ProfileId,
                ProfileName = profile.Name,
                Description = profile.Description,
                HeaderLogoPath = profile.HeaderLogoPath,
                HeaderText = profile.HeaderText,
                Logos = GetLogos(),
                Workgroup = profile.Workgroup.WorkgroupId,
                Widget = profile.Widget != null ? profile.Widget.WidgetId : -1,
                Template = profile.Template.TemplateId,
                Skills = skills,
                WorkgroupList = workgroupList,
                TemplateList = templateList,
                WidgetList = widgetList,
                SkillsList = skillsList,
                IncludeUserDataAsAttributes = profile.IncludeUserDataAsAttributes,
                IncludeUserDataAsCustomInfo = profile.IncludeUserDataAsCustomInfo,
                GeneratedScript = GenerateJavascriptForProfile(profile.Name),
                Schedules = schedules,
                SchedulesList = scheduleList,
                AllowAttachments = profile.AllowAttachments
            };
            return model;
        }

        private string GenerateJavascriptForProfile(string profileName)
        {
            var wcbDomain = "";
            try
            {
                wcbDomain = ConfigurationManager.AppSettings["WcbDomain"];
                if (!String.IsNullOrEmpty(wcbDomain))
                {
                    wcbDomain = "//" + wcbDomain;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            var generatedCode = new StringBuilder("var profile = '" + profileName + "';");
            generatedCode.AppendLine("");
            generatedCode.AppendLine("(function (window, document) {");
            generatedCode.AppendLine("    var loader = function () {");
            generatedCode.AppendLine("        var script = document.createElement('script'), tag = document.getElementsByTagName('script')[0];");
            generatedCode.AppendLine("        script.src = '" + wcbDomain + "/webchat.js?profile=' + profile;");
            generatedCode.AppendLine("        tag.parentNode.insertBefore(script, tag);");
            generatedCode.AppendLine("    };");
            generatedCode.AppendLine("window.addEventListener ? window.addEventListener('load', loader, false) : window.attachEvent('onload', loader);");
            generatedCode.AppendLine("})(window, document);");
            return generatedCode.ToString();
        }

        [Authorize(Roles = "ProfileAdmin")]
        [HttpPost]
        public ActionResult UpdateProfile(CreateProfileViewModel model)
        {
            try
            {
                var existingName = _repository.Profiles.FirstOrDefault(p => p.Name.ToLower() == model.ProfileName.ToLower() && p.ProfileId != model.ProfileId);
                var widget = _repository.Widgets.FirstOrDefault(w => w.WidgetId == model.Widget);
                var template = _repository.Templates.FirstOrDefault(t => t.TemplateId == model.Template);
                var workgroup = _repository.Workgroups.FirstOrDefault(w => w.WorkgroupId == model.Workgroup);
                var user = _repository.Users.Find(User.Identity.GetUserId());
                var skills = model.Skills != null && model.Skills.Any() ? model.Skills.Select(skillId => _repository.Skills.Find(skillId)).ToList() : null;
                var schedules = model.Schedules != null && model.Schedules.Any() ? model.Schedules.Select(i => _repository.Schedules.Find(i)).ToList() : null;

                if (existingName != null)
                {
                    ModelState.AddModelError("", "Profile Name is not Unique.");
                }
                ////Need to add this back
                //if (widget == null)
                //{
                //    ModelState.AddModelError("","Widget Not Found.");
                //}
                if (template == null)
                {
                    ModelState.AddModelError("", "Template Not Found.");
                }
                if (workgroup == null)
                {
                    ModelState.AddModelError("", "Workgroup Not Found.");
                }
                if (ModelState.IsValid)
                {
                    var profile = _repository.Profiles.Find(model.ProfileId);
                    if (profile != null)
                    {
                        profile.Name = model.ProfileName;
                        profile.HeaderLogoPath = model.HeaderLogoPath;
                        profile.HeaderText = model.HeaderText;
                        profile.Description = model.Description;
                        profile.Template = template;
                        profile.Widget = widget;
                        profile.Workgroup = workgroup;
                        profile.Skills.Clear();
                        profile.Skills = skills;
                        profile.LastUpdatedBy = user;
                        profile.LastUpdatedOn = DateTime.Now;
                        profile.IncludeUserDataAsAttributes = model.IncludeUserDataAsAttributes;
                        profile.IncludeUserDataAsCustomInfo = model.IncludeUserDataAsCustomInfo;
                        profile.Schedules.Clear();
                        profile.Schedules = schedules;
                        profile.AllowAttachments = model.AllowAttachments;
                    }
                    _repository.SaveChanges();
                    UpdateSchedules(profile);
                    ChatServices.UpdateRefreshWatch();
                    return Json(new { success = true, message = "Profile updated successfully!" });
                }
            }
            catch (Exception)
            {

            }
            model.Logos = GetLogos();
            var workgroupList = _repository.Workgroups.Where(w => w.IsAssignable).Select(w => new SelectListItem
            {
                Text = w.DisplayName,
                Value = w.WorkgroupId.ToString(),
                Selected = w.WorkgroupId == model.Workgroup
            }).ToList();
            var templateList = _repository.Templates.Select(t => new SelectListItem
            {
                Text = t.Title,
                Value = t.TemplateId.ToString(),
                Selected = t.TemplateId == model.Template
            }).ToList();
            var widgetList = !_repository.Widgets.Any() ? new List<SelectListItem>() : _repository.Widgets.Select(w => new SelectListItem
            {
                Text = w.Name,
                Value = w.WidgetId.ToString(),
                Selected = w.WidgetId == model.Widget
            }).ToList();
            var skillsList = !_repository.Skills.Any(s => s.IsAssignable) ? new List<Skill>() : _repository.Skills.Where(s => s.IsAssignable).ToList();

            var schedulesList = !_repository.Schedules.Any(s => s.IsAssignable) ? new List<Schedule>() : _repository.Schedules.Where(s => s.IsAssignable).ToList();

            model.SchedulesList = schedulesList;
            model.SkillsList = skillsList;
            model.WorkgroupList = workgroupList;
            model.TemplateList = templateList;
            model.WidgetList = widgetList;
            return PartialView("_CreateProfile", model);
        }

        [Authorize(Roles = "ProfileAdmin")]
        [HttpGet]
        public ActionResult GetDeleteProfile(int profileId)
        {
            var profile = _repository.Profiles.Find(profileId);
            if (profile != null)
            {
                var model = new ProfileViewModel
                {
                    ProfileId = profile.ProfileId,
                    ProfileName = profile.Name
                };
                return PartialView("_DeleteProfile", model);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "ProfileAdmin")]
        [HttpPost]
        public ActionResult DeleteProfile(int profileId)
        {
            var success = false;
            var message = "";
            try
            {
                var profile = _repository.Profiles.Find(profileId);
                if (profile != null)
                {
                    profile.Widget = null;
                    profile.Template = null;
                    profile.Workgroup = null;
                    profile.Skills.Clear();
                    profile.Schedules.Clear();
                    _repository.SaveChanges();
                    _repository.Profiles.Remove(profile);
                    _repository.SaveChanges();
                    success = true;
                    message = "Profile Deleted Successfully.";
                    ChatServices.UpdateRefreshWatch();
                }
                else
                {
                    message = "Profile not found.";
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Json(new { success, message });
        }

        [Authorize(Roles = "ProfileAdmin")]
        [HttpGet]
        public ActionResult CheckAvailability(int profileId)
        {
            try
            {
                var hasSchedule = false;
                var message = "";
                var closed = true;
                DateTime? now = null;
                DateTime? start = null;
                DateTime? end = null;
                var hasProfileAvailability = false;
                var scheduleActiveRanges = 0;

                var chatProfile = _repository.Profiles.Find(profileId);
                if (chatProfile != null)
                {
                    if (chatProfile.Schedules.Any(i => i.IsActive && !i.MarkedForDeletion))
                    {
                        hasSchedule = true;
                        try
                        {
                            if (!ScheduleManager.ProfileAvailabilities.Any(i => i.ProfileId == chatProfile.ProfileId))
                            {
                                ScheduleManager.LoadScheduleForProfile(chatProfile);
                            }

                            now = DateTime.UtcNow.AddHours(ScheduleManager.ScheduleDateTimeOffset);

                            var profileAvailability = ScheduleManager.ProfileAvailabilities.FirstOrDefault(i => i.ProfileId == chatProfile.ProfileId);
                            if (profileAvailability != null)
                            {
                                hasProfileAvailability = true;
                                scheduleActiveRanges = profileAvailability.ScheduleActiveRanges.Count();

                                if (profileAvailability.ScheduleActiveRanges.Any(i => now >= i.StartDateTime && now <= i.EndDateTime))
                                {
                                    var s = profileAvailability.ScheduleActiveRanges.Where(i => now >= i.StartDateTime && now <= i.EndDateTime).OrderBy(o => o.SchedulePriority).FirstOrDefault();
                                    if (s != null)
                                    {
                                        message = s.Message;
                                        end = s.EndDateTime;
                                        start = s.StartDateTime;
                                        if (!s.IsClosed)
                                        {
                                            closed = false;
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            LoggingService.GetInstance().LogException(e);
                        }
                    }

                    var model = new ProfileCheckViewModel
                    {
                        Closed = closed.ToString(),
                        HasSchedule = hasSchedule.ToString(),
                        Message = message,
                        TimeNow = now.HasValue ? now.Value.ToString() : null,
                        HasProfileAvailability = hasProfileAvailability.ToString(),
                        ScheduleActiveRanges = scheduleActiveRanges.ToString(),
                        StartTime = start.HasValue ? start.Value.ToShortTimeString() : null,
                        EndTime = end.HasValue ? end.Value.ToShortTimeString() : null,
                    };
                    return PartialView("_CheckProfile", model);
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        private ICollection<string> GetLogos()
        {
            try
            {
                return Directory.EnumerateFiles(Server.MapPath("~/Images/Logos")).Select(fn => "/Images/Logos/" + Path.GetFileName(fn)).ToList();
            }
            catch (Exception)
            {
                return new Collection<string>();
            }
        }
    }
}