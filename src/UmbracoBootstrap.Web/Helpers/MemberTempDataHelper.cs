using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UmbracoBootstrap.Web.Helpers
{
    public class MemberTempDataHelper
    {
        private TempDataDictionary TempData { get; }

        public MemberTempDataHelper(TempDataDictionary tempDataDictionary)
        {
            TempData = tempDataDictionary;
        }

        /// <summary>
        /// Return the email the mail message was sent to
        /// </summary>
        public string ConfirmEmailMailSent
        {
            get {
                return TempData.GetValue<string>("ConfirmEmailMailSent");
            }
        }

        public bool ApprovalRequestMailSent
        {
            get { return TempData.GetValue<bool>("ApprovalRequestMailSent"); }
            set { TempData["ApprovalRequestMailSent"] = value; }
        }

        public bool ForgotPasswordMailSent
        {
            get { return TempData.GetValue<bool>("ForgotPasswordMailSent"); }
            set { TempData["ForgotPasswordMailSent"] = value; }
        }

        /// <summary>
        /// Approval request has been resent
        /// </summary>
        public string ResendApprovalRequestSuccess {
            get { return TempData.GetValue<string>("ResendConfirmAccountMailSuccess"); }
            set { TempData["ResendConfirmAccountMailSuccess"] = value; }
        }

        /// <summary>
        /// Indicates that an account by specified data value was not found.
        /// </summary>
        public bool AccountNotFound {
            get { return TempData.GetValue<bool>("AccountNotFound"); }
            set { TempData["AccountNotFound"] = value; }
        }

        /// <summary>
        /// Indicates that an account by specified data value was not found.
        /// </summary>
        public bool EmailNotFound
        {
            get { return TempData.GetValue<bool>("EmailNotFound"); }
            set { TempData["EmailNotFound"] = value; }
        }

        /// <summary>
        /// Indicates that the profile has been registered
        /// </summary>
        public string RegisterProfileSuccess {
            get { return TempData.GetValue<string>("RegisterProfileSuccess"); }
            set { TempData["RegisterProfileSuccess"] = value; }
        }

        /// <summary>
        /// Indicates that the password has been changed
        /// </summary>
        public bool PasswordChanged
        {
            get { return TempData.GetValue<bool>("PasswordChangedSuccess"); }
            set { TempData["PasswordChangedSuccess"] = value; }
        }

        /// <summary>
        /// Profile username changed
        /// </summary>
        public bool ProfileUsernameChanged
        {
            get { return TempData.GetValue<bool>("ProfileUsernameChanged"); }
            set { TempData["ProfileUsernameChanged"] = value; }
        }

        /// <summary>
        /// Profile date was saved successfully
        /// </summary>
        public bool ProfileUpdateSuccess
        {
            get { return TempData.GetValue<bool>("ProfileUpdateSuccess"); }
            set { TempData["ProfileUpdateSuccess"] = value; }
        }

        public bool AccountDeleted
        {
            get { return TempData.GetValue<bool>("AccountDeleted"); }
            set { TempData["AccountDeleted"] = value; }
        }

        public bool AccountHasBeenApproved
        {
            get { return TempData.GetValue<bool>("AccountHasBeenApproved"); }
            set { TempData["AccountHasBeenApproved"] = value; }
        }
    }
}
