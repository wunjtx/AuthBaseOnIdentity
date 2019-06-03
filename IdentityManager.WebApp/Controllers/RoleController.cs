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
    public class RoleController : Controller
    {
        private UserManager _userManager;
        public UserManager UserManager
        {
            get
            {
                return _userManager ??
                HttpContext.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private RoleManager _roleManager;
        public RoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<RoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpGet]
        public ActionResult List()
        {
            var roleList = RoleManager.Roles.ToList();
            return View(roleList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await RoleManager.CreateAsync(new Role(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("List");
                }
                AddErrors(result);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Role role = RoleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var editModel = new EditRoleViewModel()
            {
                Name = role.Name
            };
            return View(editModel);
        }

        [HttpGet]
        public async Task<ActionResult> Del(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var result = await RoleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                AddErrors(result);
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, EditRoleViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(id))
            {
                var role = RoleManager.FindById(id);
                if (role==null)
                {
                    return HttpNotFound();
                }
                role.Name = model.Name;
                var result = await RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("List");
                }
                AddErrors(result);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult AddUserToRole(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(id);
            if (role==null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = id;
            var memberIDs = role.Users.Select(x => x.UserId).ToArray();
            var members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            var membersNo = UserManager.Users.Except(members).ToList();
            List<ShowUserViewModel> users = new List<ShowUserViewModel>();
            membersNo.ForEach(m =>
            {
                var user = new ShowUserViewModel();
                user.Id = m.Id;
                user.UserName = m.UserName;
                user.Email = m.Email;
                user.FBAccount = m.FBAccount;
                user.TwitterAccount = m.TwitterAccount;
                users.Add(user);
            });
            return View(users);
        }

        public ActionResult AddToRole(string userId, string roleName, string roleId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(roleName))
            {
                var result = UserManager.AddToRole(userId, roleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("AddUserToRole", new { id = roleId });
                }
            }
            return View("Error");
        }

        public ActionResult ViewRoleUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(id);
            ViewBag.RoleName = role.Name;
            var memberIDs = role.Users.Select(x => x.UserId).ToArray();
            var members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id)).ToList();
            List<ShowUserViewModel> users = new List<ShowUserViewModel>();
            members.ForEach(m =>
            {
                var user = new ShowUserViewModel();
                user.Id = m.Id;
                user.UserName = m.UserName;
                user.Email = m.Email;
                user.FBAccount = m.FBAccount;
                user.TwitterAccount = m.TwitterAccount;
                users.Add(user);
            });
            return View(users);
        }

        [HttpGet]
        public ActionResult RemoveRoleUser(string roleName, string userId)
        {
            if (string.IsNullOrWhiteSpace(roleName) && string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var result = UserManager.RemoveFromRole(userId, roleName);
            var role = RoleManager.FindByName(roleName);
            if (!result.Succeeded || role==null)
            {
                return View("Error");
            }
            return RedirectToAction("ViewRoleUser", new { id = role.Id });
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