using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdentityManager.WebApp.Controllers
{
    
    public class AuthorizeTestController : Controller
    {
        // GET: Authorize
        [Authorize(Users = "1122,admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Contact()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Home()
        {
            return View();
        }
    }
}