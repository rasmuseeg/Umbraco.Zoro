using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Web.DataAnnotations;

namespace UmbracoBootstrap.Web.Models
{
    public class MemberConsts {
        public const int PasswordMinLength = 4;
        public const int PasswordMaxLength = 32;
    }

    public class LogoutModel
    {
        public string RedirectUrl { get; set; }
    }

    public class LoginModel
    {
        [UmbracoDisplayName(nameof(Username)),
            UmbracoRequired]
        public string Username { get; set; }

        [UmbracoDisplayName(nameof(Password)),
            UmbracoStringLength(MemberConsts.PasswordMaxLength, MinimumLength = MemberConsts.PasswordMinLength),
            UmbracoRequired(),
            DataType(DataType.Password)]
        public string Password { get; set; }

        public string RedirectUrl { get; set; }
    }

    public class ForgotPasswordModel
    {
        [UmbracoRequired]
        public int MemberId { get; set; }
        public string Email { get; set; }

        [UmbracoDisplayName(nameof(Password)),
            UmbracoStringLength(MemberConsts.PasswordMaxLength, MinimumLength = MemberConsts.PasswordMinLength),
            UmbracoRequired(),
            DataType(DataType.Password)]
        public string Password { get; set; }

        [UmbracoDisplayName(nameof(ConfirmPassword)),
            UmbracoRequired(),
            UmbracoCompare(nameof(Password)),
            DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordResetModel
    {
        [UmbracoRequired]
        public int MemberId { get; set; }

        [UmbracoDisplayName(nameof(Password)),
            UmbracoStringLength(MemberConsts.PasswordMaxLength, MinimumLength = MemberConsts.PasswordMinLength),
            UmbracoRequired(),
            DataType(DataType.Password)]
        public string Password { get; set; }

        [UmbracoDisplayName(nameof(ConfirmPassword)),
            UmbracoRequired(),
            UmbracoCompare(nameof(Password)),
            DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordModel
    {
        [UmbracoRequired]
        [UmbracoDisplayName(nameof(Password))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [UmbracoDisplayName(nameof(ComparePassword))]
        [UmbracoCompare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ComparePassword { get; set; }
    }

    public class ProfileModel
    {
        public int Id { get; set; }

        [UmbracoRequired]
        [UmbracoDisplayName(nameof(FirstName))]
        public string FirstName { get; set; }

        [UmbracoRequired]
        [UmbracoDisplayName(nameof(LastName))]
        public string LastName { get; set; }

        [UmbracoRequired]
        [UmbracoDisplayName(nameof(Email))]
        [DataType(DataType.EmailAddress)]
        [EmailAddress()]
        public string Email { get; set; }

        [UmbracoDisplayName(nameof(Photo))]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Photo { get; set; }

        public string PhotoUrl { get; set; }

        public string FullName
        {
            get { return this.FirstName.Trim() + " " + this.LastName.Trim(); }
        }

        public string Username { get; internal set; }
    }

    public class RegisterModel
    {
        [UmbracoDisplayName(nameof(Photo))]
        public HttpPostedFileBase Photo { get; set; }

        public string PhotoUrl { get; set; }

        [UmbracoDisplayName(nameof(FirstName))]
        [UmbracoRequired()]
        public string FirstName { get; set; }

        [UmbracoDisplayName(nameof(LastName))]
        [UmbracoRequired()]
        public string LastName { get; set; }

        private string _email;
        [UmbracoDisplayName(nameof(Email))]
        [UmbracoRequired()]
        [UmbracoEmailAddress]
        public string Email
        {
            get { return _email; }
            set { _email = value.ToLower().Trim(); }
        }

        [UmbracoDisplayName(nameof(Password)),
            // Validation must be equal to whats defined the the web.config under the MembersMembershipProvider
            UmbracoStringLength(MemberConsts.PasswordMaxLength, MinimumLength = MemberConsts.PasswordMinLength),
            DataType(DataType.Password)]
        public string Password { get; set; }

        [UmbracoDisplayName(nameof(ConfirmPassword))]
        [UmbracoCompare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [UmbracoDisplayName(nameof(TermsAndConditions))]
        public bool TermsAndConditions { get; set; }

        public string FullName
        {
            get { return this.FirstName.Trim() + " " + this.LastName.Trim(); }
        }

        public string RedirectUrl { get; set; }
    }
}
