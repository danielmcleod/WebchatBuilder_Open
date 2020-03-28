using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebchatBuilder.Helpers;
using WebchatBuilder.Services;
using WebchatBuilder.ViewModels;
using WebChatBuilderModels.Models;

namespace WebchatBuilder.Controllers
{
    [AuthorizeIpAddress]
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly string _overrideAdminAuth = ConfigurationManager.AppSettings["OverrideAdminAuth"];
        private bool _hasCheckedOverride = false;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
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


        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!_hasCheckedOverride)
            {
                _hasCheckedOverride = true;
                if (!String.IsNullOrWhiteSpace(_overrideAdminAuth))
                {
                    ChangeAdminAuth();
                }
            }
            return View();
        }


        private void ChangeAdminAuth()
        {
            try
            {
                var user = UserManager.FindByName("WcbAdmin");
                if (user != null)
                {
                    UserManager.RemovePassword(user.Id);
                    UserManager.AddPassword(user.Id, _overrideAdminAuth);
                }
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogException(e);
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindByName(model.UserName);
            if (user != null)
            {
                if (!user.IsActive || user.IsDeleted)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
 
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Edit
        [HttpGet]
        public ActionResult Edit()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var imgPath = String.IsNullOrEmpty(user.DisplayImage) ? "/Content/Images/WcbUser.png" : user.DisplayImage;
            var model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                ImgPath = imgPath,
                Title = user.Title,
                IsActive = user.IsActive,
                UserId = user.Id
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {
            if (ModelState.IsValid && model.UserId == User.Identity.GetUserId())
            {
                var user = UserManager.FindById(model.UserId);
                if (user != null)
                {
                    user.DisplayName = model.DisplayName;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.DateUpdated = DateTime.Now;
                    user.Title = model.Title;
                    var result = UserManager.Update(user);
                    
                    return RedirectToAction("Edit");
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUserImage(string userId = "")
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/Users"), fileName);
                        var isUnique = false;
                        var cnt = 0;
                        while (!isUnique)
                        {
                            if (!System.IO.File.Exists(path))
                            {
                                isUnique = true;
                            }
                            else
                            {
                                cnt++;
                                var filePartArray = fileName.Split('.');
                                var partCnt = filePartArray.Count();
                                filePartArray[partCnt - 2] = filePartArray[partCnt - 2] + cnt;
                                var newFileName = "";
                                foreach (var part in filePartArray)
                                {
                                    newFileName += part;
                                }
                                path = Path.Combine(Server.MapPath("~/Images/Users"), newFileName);
                                fileName = newFileName;
                            }
                        }

                        file.SaveAs(path);
                        if (String.IsNullOrEmpty(userId))
                        {
                            var user = UserManager.FindById(User.Identity.GetUserId());
                            user.DisplayImage = "/Images/Users/" + fileName;
                            UserManager.Update(user);
                        }
                        else
                        {
                            var user = UserManager.FindById(userId);
                            user.DisplayImage = "/Images/Users/" + fileName;
                            UserManager.Update(user);
                        }
                        
                        return Json(new { success = true, imgUrl = "/Images/Users/" + fileName });
                    }
                }
            }
            catch (Exception e)
            {

            }

            return Json(new { success = false });
        }

        [Authorize(Roles = "UserAdmin")]
        public ActionResult GetUserInfo(string userId)
        {
            try
            {
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    var model = new UserViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DisplayName = user.DisplayName,
                        Title = user.Title
                    };
                    return PartialView("_EditUser", model);
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public ActionResult UpdateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.UserId);
                if (user != null)
                {
                    user.DisplayName = model.DisplayName;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.DateUpdated = DateTime.Now;
                    user.Title = model.Title;
                    var result = UserManager.Update(user);

                    return Json(new {success = true});
                }
            }
            return PartialView("_EditUser", model);
        }

        [Authorize(Roles = "UserAdmin")]
        public ActionResult GetPwdForm(string userId)
        {
            try
            {
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    var model = new PwdViewModel
                    {
                        UserId = user.Id
                    };
                    return PartialView("_ChangePassword", model);
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public ActionResult ChangePassword(PwdViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.UserId);
                if (user != null)
                {
                    if (user.UserName == "WcbAdmin")
                    {
                        ModelState.AddModelError("ChangePasswordError","You cannot change the password for this user.");
                    }
                    else
                    {
                        UserManager.RemovePassword(user.Id);
                        UserManager.AddPassword(user.Id, model.Password);

                        return Json(new { success = true });
                    }
                }
            }
            return PartialView("_ChangePassword", model);
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public JsonResult AddUserToRole(string roleName, string userId)
        {
            try
            {
                var role = RoleManager.FindByName(roleName);
                if (role != null)
                {
                    var user = UserManager.FindById(userId);
                    if (user != null)
                    {
                        UserManager.AddToRole(userId, role.Name);
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public JsonResult RemoveUserFromRole(string roleId, string userId)
        {
            try
            {
                var role = RoleManager.FindById(roleId);
                if (role != null)
                {
                    var user = UserManager.FindById(userId);
                    if (user != null && user.UserName != "WcbAdmin")
                    {
                        UserManager.RemoveFromRole(userId, role.Name);
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "UserAdmin")]
        public JsonResult GetRoleAdmins(string roleName)
        {
            try
            {
                var role = RoleManager.FindByName(roleName);
                if (role != null)
                {
                    var userList = role.Users.Select(u => UserManager.FindById(u.UserId)).Where(user => user != null).ToList();
                    if (userList.Any())
                    {
                        var users = userList.Select(u => new
                        {
                            UserId = u.Id,
                            RoleId = role.Id,
                            DisplayName = u.DisplayName
                        });
                        return Json(new { success = true, UserList = users }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "UserAdmin")]
        public ActionResult GetNewUserForm()
        {
            var model = new NewUserViewModel();
            return PartialView("_CreateUser", model);
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public ActionResult CreateUser(NewUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.UserName,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Title = model.Title,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        DisplayName = model.DisplayName,
                        IsActive = true
                    };
                    var result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true });
                    }
                    AddErrors(result);
                }
            }
            catch (Exception)
            {
            }

            return PartialView("_CreateUser", model);
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public ActionResult ToggleActive(string userId, bool isActive)
        {
            try
            {
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    user.IsActive = isActive;
                    var result = UserManager.Update(user);

                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public ActionResult DeleteUser(string userId)
        {
            try
            {
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    if (user.Roles.Any())
                    {
                        var roles = UserManager.GetRoles(user.Id);
                        UserManager.RemoveFromRoles(user.Id, roles.ToArray());
                    }
                    user.IsDeleted = true;
                    var result = UserManager.Update(user);

                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
            }
            return Json(new { success = false });
        }

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        //
        // POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}



        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}