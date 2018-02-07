﻿using NordicCreativeLeadership.Web.Helpers;
using NordicCreativeLeadership.Web.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedCache;
using ContentModels = NordicCreativeLeadership.Web.PublishedContentModels;

namespace UmbracoBootstrap.Web.Controllers
{
    public class MemberController : SurfaceController
    {
        private const string SECURITYKEY_TIMESTAMP_ALIAS = "umbracoMemberSecurityKeyTimestamp";
        private const string SECURITY_KEY_ALIAS = "umbracoMemberSecurityKey";
        private const string SECURITY_CODE_ALIAS = "umbracoMemberSecurityCode";

        public MemberController()
        {
        }

        [HttpPost]
        public ActionResult HandleLogin([Bind(Prefix = "loginModel")]LoginModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            string message = "";

            var member = Services.MemberService.GetByUsername(model.Username);
            if (member != null)
            {
                if (member.IsLockedOut)
                {
                    message = string.Format(Umbraco.GetDictionaryValue("AccountLockoutError"), Membership.MaxInvalidPasswordAttempts);
                    ModelState.AddModelError("loginModel", message);
                    LogHelper.Info<MemberController>(message);
                    return CurrentUmbracoPage();
                }
                if (member.IsApproved == false)
                {
                    message = Umbraco.GetDictionaryValue("AccountNotApprovedError");
                    ModelState.AddModelError("loginModel", message);
                    LogHelper.Info<MemberController>(message);
                    return CurrentUmbracoPage();
                }
            }

            // TODO: Implement remember me / persistence functionality

            if (Members.Login(model.Username, model.Password))
            {
                if (!string.IsNullOrEmpty(model.RedirectUrl))
                {
                    return Redirect(model.RedirectUrl);
                }

                return RedirectToCurrentUmbracoUrl();
            }

            message = Umbraco.GetDictionaryValue("AccountCredentialsError");
            ModelState.AddModelError("loginModel", message);
            LogHelper.Info<MemberController>(message);

            return CurrentUmbracoPage();
        }

        public ActionResult HandleLogout([Bind(Prefix = "logoutModel")]LogoutModel model)
        {
            if (Members.IsLoggedIn())
            {
                Members.Logout();
            }

            return Redirect(model.RedirectUrl);
        }

        [MemberAuthorize]
        [ChildActionOnly]
        public ActionResult EditProfile(int id = 0)
        {
            if (id <= 0)
            {
                id = Members.GetCurrentMemberId();
            }

            ContentModels.Member member = (ContentModels.Member)Umbraco.TypedMember(id);

            var model = new ProfileModel
            {
                Id = member.Id,
                Email = member.Email.ToLower(),
                Username = member.UserName,
                FirstName = member.FirstName,
                LastName = member.LastName,
                PhotoUrl = member.GetPropertyValue<string>("photo")
            };

            return PartialView("~/Views/Members/Edit.cshtml", model);
        }

        [MemberAuthorize]
        [HttpPost]
        public ActionResult HandleEditProfile(ProfileModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            var member = Services.MemberService.GetById(model.Id);
            // save current username for later
            string userName = member.Username;
            // Should we remove photo?
            // Current member has photo, and it have been removed on frontend.
            if (string.IsNullOrWhiteSpace(member.GetValue<string>("photo")) == false
                && string.IsNullOrWhiteSpace(model.PhotoUrl) == true)
            {
                member.SetValue("photo", string.Empty);
            }

            // Upload new photo
            if (model.Photo != null && model.Photo.ContentLength > 0)
            {
                member.SetValue("photo", model.Photo);
            }

            member.Name = model.FullName;
            member.Email = model.Email;
            member.Username = model.Username;

            Services.MemberService.Save(member);

            if (userName.Equals(model.Username) == false)
            {
                TempData["ProfileUsernameChanged"] = true;
                Members.Logout();
            }

            TempData["ProfileUpdateSuccess"] = true;

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [MemberAuthorize]
        public ActionResult HandleChangePassword([Bind(Prefix = "changePasswordModel")]ChangePasswordModel model)
        {
            var memberId = Members.GetCurrentMemberId();

            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            // Member most be logged in, redundant since MemberAuthorize should catch before this.
            if (memberId <= -1)
                return RedirectToCurrentUmbracoPage();

            var member = Services.MemberService.GetById(memberId);
            Services.MemberService.SavePassword(member, model.Password);

            TempData["PasswordUpdatedSuccces"] = true;

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [NotChildAction]
        public async Task<ActionResult> HandleRegister([Bind(Prefix = "registerModel")]RegisterModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            // Check if a member exist with that email
            if (Services.MemberService.Exists(model.Email))
            {
                var message = Umbraco.GetDictionaryValue("RegisterMemberExistError", "RegisterMemberExistError");
                ModelState.AddModelError(nameof(model.Email), new Exception(message));
            }

            // Create the new member
            var member = Services.MemberService.CreateMember(model.Email, model.Email, model.FullName, "member");

            // Upload photo if any
            if (model.Photo != null && model.Photo.ContentLength > 0)
            {
                member.SetValue("photo", model.Photo);
            }

            // Save as unapproved
            member.IsApproved = false;
            member.SetValue("firstName", model.FirstName.Trim());
            member.SetValue("lastName", model.LastName.Trim());
            Services.MemberService.Save(member);

            // Try save password
            try
            {
                Services.MemberService.SavePassword(member, model.Password);
            }
            catch (Exception ex)
            {
                // Propperly a validation error
                ModelState.AddModelError(nameof(model.Password), ex);
                return CurrentUmbracoPage();
            }

            // Send confirmation email

            var redirectPage = CurrentPage.Parent.FirstChild<ContentModels.Activate>();
            string redirectUrl = redirectPage.UrlAbsolute();
            redirectUrl = await SendConfirmAccountMail(member, redirectUrl);

            TempData["RegisterSuccess"] = member.Email;

            return Redirect(redirectUrl);
        }

        [HttpPost]
        public async Task<ActionResult> HandleResendConfirmAccountMail(string email)
        {
            var member = Services.MemberService.GetByEmail(email);
            if (member == null)
            {
                ModelState.AddModelError("", Umbraco.GetDictionaryValue("AccountNotFound", "AccountNotFound"));
                TempData["ResendConfirmAccountMailError"] = Umbraco.GetDictionaryValue("AccountNotFound", "AccountNotFound");
                return CurrentUmbracoPage();
            }

            string redirectUrl = CurrentPage.UrlAbsolute(); // CurrentPage should be AccountActivatePage
            await SendConfirmAccountMail(member, redirectUrl);

            TempData["ResendConfirmAccountMailSuccess"] = member.Email;

            return RedirectToCurrentUmbracoUrl();
        }

        private async Task<string> SendConfirmAccountMail(Umbraco.Core.Models.IMember member, string redirectUrl)
        {
            try
            {
                string securityKey = GenerateSecurityKeyOnMember(member);
                string securityCode = GenerateSecurityCodeOnMember(member);

                var model = new ConfirmAccountMailModel()
                {
                    Member = (ContentModels.Member)Umbraco.TypedMember(member.Id),
                    Permalink = $"{redirectUrl}?email={Url.Encode(member.Email)}&key={securityKey}",
                    SecurityCode = securityCode
                };

                await MailMessageHelper.Current.SendMailMessageAsync(member.Email, "ConfirmAccountMail", model, this.ControllerContext);

                LogHelper.Info<MemberController>("Register Confirmation Email sent to: {0}", () => member.Email);

                return model.Permalink;
            }
            catch (Exception ex)
            {
                LogHelper.Error<MemberController>("Register Confirmation Email was not sent to: " + member.Email, ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult HandleConfirmAccount([Bind(Prefix = "confirmAccountModel")]ConfirmAccountModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            // Get member by email
            var member = Services.MemberService.GetByEmail(model.Email);
            if (member == null)
            {
                LogHelper.Error<MemberController>("Unable to find member by email.", new NullReferenceException("member"));
                ModelState.AddModelError("", Umbraco.GetDictionaryValue("AccountNotFound", "AccountNotFound"));
                LogHelper.Info<MemberController>("AccountNotFound");
                return CurrentUmbracoPage();
            }

            // Make sure security key is a match
            //if (!member.GetValue<string>(SECURITY_KEY_ALIAS).Equals(model.SecurityKey))
            //{
            //    ModelState.AddModelError("", Umbraco.GetDictionaryValue("InvalidSecurityKey", "InvalidSecurityKey"));
            //    LogHelper.Info<MemberController>("InvalidSecurityKey");
            //    return CurrentUmbracoPage();
            //}

            // Validate activation code
            string securityCode = member.GetValue<string>(SECURITY_CODE_ALIAS);
            if (!securityCode.Equals(model.SecurityCode.ToUpper()))
            {
                ModelState.AddModelError(nameof(model.SecurityCode), Umbraco.GetDictionaryValue("InvalidSecurityCode"));
                LogHelper.Info<MemberController>("InvalidSecurityCode");
                return CurrentUmbracoPage();
            }

            member.IsApproved = true;
            Services.MemberService.Save(member);
            Services.MemberService.AssignRole(member.Id, MemberGroups.Confirmed);

            TempData["ConfirmAccountSuccess"] = true;
            LogHelper.Info<MemberController>("Account has been confirmed: {0}", () => member.Email);
            return Redirect(model.RedirectUrl);
        }

        public async Task<ActionResult> HandleDeleteAccount([Bind(Prefix = "deleteAccountModel")]DeleteAccountModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByEmail(model.Email);
            if (member == null)
            {
                ModelState.AddModelError("deleteAccountModel", Umbraco.GetDictionaryValue("AccountNotFound", "AccountNotFound"));
                return View(model);
            }
            Services.MemberService.Delete(member);

            string email = member.Email;
            string subject = "";
            string viewPath = "~/Views/Email/DeleteAccountEmail.cshtml";
            var deleteAccountMailModel = new DeleteAccountMailModel()
            { 
            };
            await SendMailMessage(email, subject, viewPath, deleteAccountMailModel);

            TempData["DeleteAccountSuccess"] = true;

            return Redirect("/");
        }



       

        [HttpPost]
        [NotChildAction]
        public ActionResult HandleForgotPassword([Bind(Prefix = "forgotPasswordModel")]ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            var member = Services.MemberService.GetByEmail(model.Email);

            if (member == null)
            {
                TempData["ForgotPasswordError"] = true;
                return RedirectToCurrentUmbracoPage();
            }
            string key = GenerateSecurityKeyOnMember(member);

            var passwordResetConfirmationMailModel = new PasswordResetConfirmationMailModel()
            {
                Member = (ContentModels.Member)Umbraco.TypedMember(member.Id),
                Permalink = CurrentPage.UrlAbsolute() + "?key=" + key + "&email=" + Url.Encode(member.Email)
            };

            MailMessageHelper.Current.SendMailMessage(
                member.Email,
                "PasswordResetConfirmation",
                passwordResetConfirmationMailModel,
                 this.ControllerContext
            );

            TempData["ForgotPasswordSuccess"] = true;

            return RedirectToCurrentUmbracoUrl();
        }



        [ChildActionOnly]
        public ActionResult ResetPassword(string key, string email)
        {
            var matches = Services.MemberService.GetMembersByPropertyValue(SECURITY_KEY_ALIAS, key);
            if (matches.Count() <= 0)
            {
                TempData["InvalidKey"] = true;
                return CurrentUmbracoPage();
            }

            var member = matches.FirstOrDefault(p => p.Email == email);
            if (member == null)
            {
                TempData["InvalidEmail"] = true;
                return CurrentUmbracoPage();
            }

            var model = new PasswordResetModel()
            {
                MemberId = member.Id
            };

            return PartialView("~/Views/Members/ResetPassword.cshtml", model);
        }

        [HttpPost]
        public ActionResult HandleResetPassword(PasswordResetModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            var member = Services.MemberService.GetById(model.MemberId);
            if (member == null)
                throw new NullReferenceException("member");

            try
            {
                Services.MemberService.SavePassword(member, model.Password);

                MailMessageHelper.Current.SendMailMessage(
                    member.Email,
                    "PasswordChangedMail", 
                    CurrentPage, 
                    this.ControllerContext
                );
            }
            catch (Exception ex)
            {
                // Propperly a validation error
                ModelState.AddModelError(nameof(model.Password), ex);
                return CurrentUmbracoPage();
            }

            TempData["PasswordChanged"] = true;

            return RedirectToCurrentUmbracoPage();
        }

        private async Task SendMailMessage(string email, string subject, string viewPath, object model)
        {
            var notificationsEmail = MailMessageHelper.Current.NotificationsEmailAddress;

            using (var message = new MailMessage(notificationsEmail, email))
            {
                message.Subject = subject;
                message.Body = ViewHelper.RenderPartialView(viewPath, model, this.ControllerContext);
                message.IsBodyHtml = true;
                await MailMessageHelper.Current.SendMailAsync(message);
            };
        }

        private string GenerateSecurityKeyOnMember(Umbraco.Core.Models.IMember member)
        {
            string key = member.GetValue<string>(SECURITY_KEY_ALIAS);
            string code = member.GetValue<string>(SECURITY_CODE_ALIAS);

            DateTime timestamp = member.GetValue<DateTime>(SECURITYKEY_TIMESTAMP_ALIAS);

            if (!string.IsNullOrEmpty(key)
                && (timestamp - DateTime.UtcNow).TotalMinutes <= 20)
            {
                // Reuse key
                return key;
            }
            else
            {
                // Generate new key, and date
                key = Guid.NewGuid().ToString();
                member.SetValue(SECURITY_KEY_ALIAS, key);
                member.SetValue(SECURITYKEY_TIMESTAMP_ALIAS, DateTime.UtcNow);
                Services.MemberService.Save(member);
            }

            return key;
        }

        private string GenerateSecurityCodeOnMember(Umbraco.Core.Models.IMember member)
        {
            string securityCode = Guid.NewGuid().ToString()
                .Replace("-", string.Empty)
                .Truncate(6, "")
                .ToUpper();

            member.SetValue(SECURITY_CODE_ALIAS, securityCode);
            Services.MemberService.Save(member);

            return securityCode;
        }
    }
}