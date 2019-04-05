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
using Zoro.WebUI.Helpers;
using Zoro.WebUI.Models;
using ContentModels = Zoro.WebUI.Models.PublishedContent;

namespace Zoro.WebUI.Controllers
{
    public class MemberController : SurfaceController
    {
        private const string SECURITYKEY_EXPIRATION_DATE_ALIAS = OurConstants.Member.SecurityKeyTimestamp;
        private const string SECURITY_KEY_ALIAS = OurConstants.Member.SecurityKey;

        public MemberTempDataHelper TempDataHelper { get; }

        public MemberController()
        {
            TempDataHelper = new MemberTempDataHelper(TempData);
        }


        [HttpPost]
        public ActionResult HandleLogin([Bind(Prefix = "loginModel")]LoginModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            string message = "";

            Umbraco.Core.Models.IMember member = null;
            // Determine if username is a email
            if (!model.Username.Contains("@"))
                member = Services.MemberService.GetByUsername(model.Username);
            else
                member = Services.MemberService.GetByEmail(model.Username.ToLower());

            if (member != null)
            {
                if (member.IsLockedOut)
                {
                    message = string.Format(Umbraco.GetDictionaryValue("AccountLockoutError", "AccountNotApprovedError"), Membership.MaxInvalidPasswordAttempts);
                    ModelState.AddModelError("loginModel", message);
                    Logger.Info<MemberController>(message);
                    return CurrentUmbracoPage();
                }
                if (member.IsApproved == false)
                {
                    message = Umbraco.GetDictionaryValue("AccountNotApprovedError", "AccountNotApprovedError");
                    ModelState.AddModelError("loginModel", message);
                    Logger.Info<MemberController>(message);
                    return CurrentUmbracoPage();
                }

                // TODO: Implement remember me / persistence functionality
                // TODO: Ensure user has given cookie consent
                if (Members.Login(member.Username, model.Password))
                {
                    if (!string.IsNullOrEmpty(model.RedirectUrl))
                    {
                        return Redirect(model.RedirectUrl);
                    }

                    return RedirectToCurrentUmbracoUrl();
                }
            }

            message = Umbraco.GetDictionaryValue("AccountCredentialsError", "AccountCredentialsError");
            ModelState.AddModelError("loginModel", message);
            Logger.Info<MemberController>(message);

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

            //ContentModels.Member member = (ContentModels.Member)Umbraco.TypedMember(id);

            var model = new ProfileModel
            {
                //Id = member.Id,
                //Email = member.Email.ToLower(),
                //FirstName = member.FirstName,
                //LastName = member.LastName,
                //PhotoUrl = member.GetPropertyValue<string>("photo")
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
            string username = member.Username;
            // Should we remove photo?
            // Current member has photo, and it have been removed on frontend.
            //if (string.IsNullOrWhiteSpace(member.GetValue<string>("photo")) == false
            //    && string.IsNullOrWhiteSpace(model.PhotoUrl) == true)
            //{
            //    member.SetValue("photo", string.Empty);
            //}

            // Upload new photo
            //if (model.Photo != null && model.Photo.ContentLength > 0)
            //{
            //    member.SetValue("photo", model.Photo);
            //}

            member.Name = model.FullName;
            member.Email = model.Email;
            member.Username = model.Username;

            Services.MemberService.Save(member);

            if (username != model.Username)
            {
                TempDataHelper.ProfileUsernameChanged = true;
                Members.Logout();
            }

            TempDataHelper.ProfileUpdateSuccess = true;

            return RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Change password after being logged in
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

            TempDataHelper.PasswordChanged = true;

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [NotChildAction]
        public async Task<ActionResult> HandleRegisterProfile([Bind(Prefix = "registerProfileModel")]RegisterProfileModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            if(model.AcceptTermsAndPrivacy == false)
            {
                var message = Umbraco.GetDictionaryValue("AcceptTermsAndPrivacyDisagreement", "AcceptTermsAndPrivacyDisagreement");
                ModelState.AddModelError(nameof(model.AcceptTermsAndPrivacy), new Exception(message));
                return CurrentUmbracoPage();
            }

            // Check if a member exist with that email,
            // TODO: This is bad, because hackers might exploit this function.
            if (Services.MemberService.Exists(model.Email))
            {
                var message = Umbraco.GetDictionaryValue("RegisterMemberExistError", "RegisterMemberExistError");
                ModelState.AddModelError(nameof(model.Email), new Exception(message));
            }

            // Create the new member
            var member = Services.MemberService.CreateMember(
                model.Email, 
                model.Email, 
                model.FullName, 
                Constants.Conventions.MemberTypes.DefaultAlias
            );

            //// Upload photo if any
            //if (model.Photo != null && model.Photo.ContentLength > 0)
            //{
            //    member.SetValue("photo", model.Photo);
            //}

            // Save as unapproved
            member.IsApproved = false; // TODO: New members must approve email address
            member.SetValue("firstName", model.FirstName.Trim());
            member.SetValue("lastName", model.LastName.Trim());
            member.SetValue("phoneNumber", model.PhoneNumber.Trim());
            member.SetValue(Constants.Conventions.Member.PasswordQuestion, model.PasswordQuestion);
            member.SetValue(Constants.Conventions.Member.PasswordAnswer, model.PasswordAnswer);
            Services.MemberService.Save(member);

            // Try save password
            try
            {
                Services.MemberService.SavePassword(member, model.Password);
            }
            catch (Exception ex)
            {
                throw new Exception("UmbracoMembershipProvider does not allow manually chaing password. Change 'allowManuallyChangingPassword' to true.", ex);
            }

            // Send confirmation email

            //var redirectPage = CurrentPage.Parent.FirstChild<ContentModels.Activate>();
            //string redirectUrl = redirectPage.UrlAbsolute();
            /// Create redirect url
            await SendApprovalRequestMail(member, CurrentPage.UrlAbsolute());

            TempDataHelper.RegisterProfileSuccess = member.Email;

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public async Task<ActionResult> SendApproveEmailMail(string email)
        {
            var member = Services.MemberService.GetByEmail(email);
            if (member == null)
            {
                ModelState.AddModelError("", Umbraco.GetDictionaryValue("AccountNotFound", "AccountNotFound"));
                TempDataHelper.AccountNotFound = true;
                return CurrentUmbracoPage();
            }

            string redirectUrl = CurrentPage.UrlAbsolute(); // CurrentPage should be AccountActivatePage
            await SendApprovalRequestMail(member, redirectUrl);

            TempDataHelper.ResendApprovalRequestSuccess = member.Email;

            return RedirectToCurrentUmbracoUrl();
        }

        /// <summary>
        /// Sends an approval request to the specified members email address.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="hostAndScheme"></param>
        /// <returns>{hostAndScheme}?email={Url.Encode(member.Email)}&key={securityKey}</returns>
        private async Task<Uri> SendApprovalRequestMail(Umbraco.Core.Models.IMember member, string hostAndScheme)
        {
            try
            {
                string securityKey = GenerateSecurityKeyOnMember(member);

                var model = new ApproveEmailMailModel()
                {
                    Member = Umbraco.TypedMember(member.Id),
                    Permalink = new Uri($"{hostAndScheme}?email={Url.Encode(member.Email)}&key={Url.Encode(securityKey)}"),
                };

                //await MailMessageHelper.Current.SendMailMessageAsync(member.Email, "ConfirmAccountMail", model, this.ControllerContext);

                Logger.Info<MemberController>("Register Confirmation Email sent to: {0}", () => member.GetValue<string>("Email"));

                TempDataHelper.ApprovalRequestMailSent = true;

                return model.Permalink;
            }
            catch (Exception ex)
            {
                Logger.Error<MemberController>("Register Confirmation Email was not sent to: " + member.Email, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Approves member by email and security code, and assigns default member role.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HandleApproveMember(ApproveMemberModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            // Get member by email
            var member = Services.MemberService.GetByEmail(model.Email);
            if (member == null)
            {
                Logger.Error<MemberController>("Email not found.", new NullReferenceException("member"));
                TempDataHelper.EmailNotFound = true;
                return CurrentUmbracoPage();
            }

            // Make sure security key is a match
            if (!member.GetValue<string>(SECURITY_KEY_ALIAS).Equals(model.SecurityKey))
            {
                ModelState.AddModelError(nameof(model.SecurityKey), Umbraco.GetDictionaryValue("InvalidSecurityKey", "InvalidSecurityKey"));
                Logger.Info<MemberController>("InvalidSecurityKey");
                return CurrentUmbracoPage();
            }

            member.IsApproved = true;
            Services.MemberService.Save(member);
            Services.MemberService.AssignRole(member.Id, OurConstants.MemberGroups.Default);

            TempDataHelper.AccountHasBeenApproved = true;

            Logger.Debug<MemberController>("Member has been approved: {0}", () => member.GetValue<string>("Email"));
            return Redirect(model.LoginPageUrl);
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
                ModelState.AddModelError("deleteAccountModel", Umbraco.GetDictionaryValue("EmailNotFound", "EmailNotFound"));
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

            TempDataHelper.AccountDeleted = true;

            Members.Logout();

            return Redirect("/");
        }

        /// <summary>
        /// Handles password change request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HandleForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            var member = Services.MemberService.GetById(model.MemberId);
            if (member == null)
                throw new NullReferenceException("member");

            try
            {
                Services.MemberService.SavePassword(member, model.Password);

                //MailMessageHelper.Current.SendMailMessage(
                //    member.Email,
                //    "PasswordChangedMail",
                //    CurrentPage,
                //    this.ControllerContext
                //);
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

                //MailMessageHelper.Current.SendMailMessage(
                //    member.Email,
                //    "PasswordChangedMail",
                //    CurrentPage,
                //    this.ControllerContext
                //);
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

            DateTime expirationDate = member.GetValue<DateTime>(SECURITYKEY_EXPIRATION_DATE_ALIAS);

            if (!string.IsNullOrEmpty(key) && expirationDate > DateTime.UtcNow)
            {
                // Reuse key
                return key;
            }
            else
            {
                expirationDate = DateTime.UtcNow.Add(MemberConfiguration.SecurityKeyExpiryDuration);
                // Generate new key, and date
                key = Guid.NewGuid().ToString();
                member.SetValue(SECURITY_KEY_ALIAS, key);
                member.SetValue(SECURITYKEY_EXPIRATION_DATE_ALIAS, expirationDate);
                Services.MemberService.Save(member);
            }

            return key;
        }
    }
}