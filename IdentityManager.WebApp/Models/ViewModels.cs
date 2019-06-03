using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdentityManager.WebApp.Models
{
    public class AddUserViewModel
    {
        [Required]
        [StringLength(64)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} must contain at least {2} characters。 ", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not match. ")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "FaceBook Account")]
        [StringLength(64)]
        public string FBAccount { get; set; }

        [Display(Name = "Twitter Account")]
        [StringLength(64)]
        public string TwitterAccount { get; set; }
    }

    public class ShowUserViewModel
    {

        [Required]
        [StringLength(64)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "FaceBook Account")]
        [StringLength(64)]
        public string FBAccount { get; set; }

        [Display(Name = "Twitter Account")]
        [StringLength(64)]
        public string TwitterAccount { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        public string Id { get; set; }
    }

    public class EditUserViewModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(64)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "FaceBook Account")]
        [StringLength(64)]
        public string FBAccount { get; set; }

        [Display(Name = "Twitter Account")]
        [StringLength(64)]
        public string TwitterAccount { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        [StringLength(64)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel 
    {
        [Required]
        [Display(Name = "User Name")]
        [StringLength(64)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} must contain at least {2} characters. ", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not match! ")]
        public string ConfirmPassword { get; set; }
        [StringLength(64)]
        public string UserName { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
    }

    public class AddRoleViewModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }

    public class EditRoleViewModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "角色名称")]
        public string Name { get; set; }
    }
}
