using IdentityManager.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IdentityManager.WebApp
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store) : base(store) { }
        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(new UserStore<User>(context.Get<Data.IdentityDbContext>()));
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            manager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("EmailConfirmation"));
            }
            return manager;
        }
    }

    public class SignInManager : SignInManager<User, string>
    {
        public SignInManager(UserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        { }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((UserManager)UserManager);
        }
        public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
        {
            return new SignInManager(context.GetUserManager<UserManager>(), context.Authentication);
        }
    }

    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(RoleStore<Role> store) : base(store) { }
        public static RoleManager Create(IdentityFactoryOptions<RoleManager> options, IOwinContext context)
        {
            var manager = new RoleManager(new RoleStore<Role>(context.Get<Data.IdentityDbContext>()));
            return manager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //For TLS:  587
            //For SSL:  465
            Common.SendMail("smtp.gmail.com", 587, "your email pwd", "your email", message.Destination, message.Subject, message.Body);
            return Task.FromResult(0);
        }
    }

    public class Common
    {
        public static bool SendMail(string strSmtpServer, int iSmtpPort, string Password, string strFrom, string strto, string strSubject, string strBody,bool enableSsl = true)
        {
            //set sender emailaddress and show name 
            MailAddress mailFrom = new MailAddress(strFrom);
            //set receiver emailaddress and show name
            MailAddress mailTo = new MailAddress(strto);
            //create a mailmessage instance
            MailMessage oMail = new MailMessage(mailFrom, mailTo);
            oMail.Subject = strSubject;
            oMail.Body = strBody;
            oMail.IsBodyHtml = true; //mail format support html
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("UTF-8");//codeing
            oMail.SubjectEncoding = System.Text.Encoding.GetEncoding("UTF-8");//coding
            oMail.Priority = MailPriority.High;//email priority

            //sender email server
            SmtpClient client = new SmtpClient();

            // email smtp
            client.Host = strSmtpServer; //set the server 

            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.EnableSsl = enableSsl;
                                         
            client.Port = iSmtpPort; //set port
            
            client.Timeout = 9999; //set time out
            
            client.UseDefaultCredentials = false; //set to credential
            
            client.Credentials = new NetworkCredential(strFrom, Password);//set sender emailaddress and pwd

            client.Send(oMail); //send mail, plz also  enable allow less secure app access in your email security setting.

            //release all source                    
            mailFrom = null;
            mailTo = null;
            client.Dispose();
            oMail.Dispose(); 
            return true;
        }
    }
}