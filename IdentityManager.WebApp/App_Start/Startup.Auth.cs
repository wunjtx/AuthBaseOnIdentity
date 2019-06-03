using IdentityManager.Data;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityManager.WebApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(IdentityDbContext.Create);

            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);
            app.CreatePerOwinContext<RoleManager>(RoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login")
            });
        }
    }
}