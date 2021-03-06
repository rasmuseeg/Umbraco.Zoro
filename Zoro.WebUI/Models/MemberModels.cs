﻿using Our.Umbraco.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Zoro.WebUI.Models
{
    public class MemberConsts
    {
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
        [UmbracoRequired,
            UmbracoPassword()]
        [UmbracoDisplayName(nameof(Password))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [UmbracoDisplayName(nameof(ConfirmPassword))]
        [UmbracoCompare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
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
        [UmbracoDisplayName(nameof(PhoneNumber))]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [UmbracoRequired]
        [UmbracoEmailAddress()]
        [UmbracoDisplayName(nameof(Email))]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[UmbracoDisplayName(nameof(Photo))]
        //[DataType(DataType.Upload)]
        //public HttpPostedFileBase Photo { get; set; }

        //public string PhotoUrl { get; set; }

        public string FullName
        {
            get { return this.FirstName.Trim() + " " + this.LastName.Trim(); }
        }

        public string Username { get; set; }
    }

    public class RegisterProfileModel
    {
        //[UmbracoDisplayName(nameof(Photo))]
        //public HttpPostedFileBase Photo { get; set; }

        //public string PhotoUrl { get; set; }

        [UmbracoRequired]
        [UmbracoDisplayName(nameof(FirstName))]
        public string FirstName { get; set; }

        [UmbracoRequired]
        [UmbracoDisplayName(nameof(LastName))]
        public string LastName { get; set; }

        [UmbracoRequired]
        [UmbracoDisplayName(nameof(PhoneNumber))]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        private string _email;
        [UmbracoDisplayName(nameof(Email))]
        [UmbracoRequired()]
        [UmbracoEmailAddress]
        public string Email
        {
            // This ensures email does not contain spaces
            get { return _email; }
            set { _email = value.ToLower().Trim(); }
        }

        [UmbracoDisplayName(nameof(Password)),
            UmbracoPassword(),
            UmbracoRequired(),
            DataType(DataType.Password)]
        public string Password { get; set; }

        [UmbracoDisplayName(nameof(ConfirmPassword)),
            UmbracoPassword(),
            UmbracoRequired(),
            UmbracoCompare(nameof(Password)),
            DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [UmbracoDisplayName(nameof(PasswordQuestion)),
            UmbracoRequired(),
            UmbracoMinLength(4)]
        public string PasswordQuestion { get; set; }

        [UmbracoDisplayName(nameof(PasswordAnswer)),
            UmbracoRequired(),
            UmbracoMinLength(4)]
        public string PasswordAnswer { get; set; }

        [UmbracoDisplayName(nameof(AcceptTermsAndPrivacy)),
            UmbracoMustBeTrue]
        public bool AcceptTermsAndPrivacy { get; set; }

        public string FullName
        {
            get { return this.FirstName.Trim() + " " + this.LastName.Trim(); }
        }

    }

    public class ApproveMemberModel
    {
        [UmbracoRequired]
        public string SecurityKey { get; set; }

        [UmbracoRequired]
        public string Email { get; set; }

        [UmbracoRequired]
        public string LoginPageUrl { get; set; }
    }

    public enum ApproveMemberResult
    {
        Approved,
        InvalidModelState,
        EmailNotFound,
        InvalidSecurityKey,
    }

    public class DeleteAccountModel
    {
        public string Email { get; set; }
    }
}
