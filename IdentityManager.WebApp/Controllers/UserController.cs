using IdentityManager.Data;
using IdentityManager.WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentityManager.WebApp.Controllers
{
    public class UserController : Controller
    {
        private UserManager _userManager;
        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private SignInManager _signInManager;
        public SignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<SignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddUserViewModel addUserModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = addUserModel.UserName,
                    Email = addUserModel.Email,
                    FBAccount = addUserModel.FBAccount,
                    TwitterAccount = addUserModel.TwitterAccount
                };
                var result = await UserManager.CreateAsync(user, addUserModel.Password);
                if (result.Succeeded)
                {
                    //log in
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser:
                    false);
                    //send email 
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "User", new {
                        userId = user.Id,
                        code = code
                    }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "To confirm your account, please click <a href =\"" + callbackUrl + "\">here</a>");
                    //return RedirectToAction("List", "User");
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            return View(addUserModel);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.UserName);
                
                if (user == null || user.Email!=model.Email || !user.EmailConfirmed)
                {
                    return View("Error");
                }
                //send email
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "User", new
                {
                    userId = user.Id,
                    code = code
                }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "To reset your passowrd, please click <a href=\"" + callbackUrl + "\">here</a>.");
                return RedirectToAction("ForgotPasswordConfirmation", "User");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailLink(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return View("Error");
            }
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null || user.Email==null || user.EmailConfirmed)
            {
                return View("Error");
            }
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "User", new
            {
                userId = user.Id,
                code = code
            }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "To confirm your account, please click <a href =\"" + callbackUrl + "\">here</a>");
            //return RedirectToAction("List", "User");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId,string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null ||user.Email!=model.Email)
            {
                return View("Error");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "User");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public ActionResult List()
        {
            var users = UserManager.Users.ToList();
            List<ShowUserViewModel> showUsers=new List<ShowUserViewModel> ();
            users.ForEach(u =>
            {
                var user = new ShowUserViewModel();
                user.Email = u.Email;
                user.FBAccount = u.FBAccount;
                user.TwitterAccount = u.TwitterAccount;
                user.UserName = u.UserName;
                user.Id = u.Id;
                showUsers.Add(user);
            });
            return View(showUsers);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var editUserViewModel = new EditUserViewModel()
            {
                Email = user.Email,
                UserName=user.UserName,
                FBAccount = user.FBAccount,
                TwitterAccount = user.TwitterAccount,
                EmailConfirmed=user.EmailConfirmed
            };
            return View(editUserViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user =await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            var editUserViewModel = new ShowUserViewModel()
            {
                Id=user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FBAccount = user.FBAccount,
                TwitterAccount = user.TwitterAccount,
                EmailConfirmed=user.EmailConfirmed,
            };
            return View(editUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, EditUserViewModel editUserViewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(id))
            {
                User user = UserManager.FindById(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.Email = editUserViewModel.Email;
                user.FBAccount = editUserViewModel.FBAccount;
                user.TwitterAccount = editUserViewModel.TwitterAccount;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Details", "User",new { userName = user.UserName}); 
                }
                AddErrors(result);
            }
            ModelState.AddModelError("", "not valid");
            return View(editUserViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // this will not lock out account but add to login fails count
            // if want to lock out account after too much fails plz change shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password,
            model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid Login Attempt。 ");
                    return View(model);
            }
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            SignInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Index","Home");
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
