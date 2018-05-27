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
        public TempDataDictionary TempData { get; }

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

    }
}
