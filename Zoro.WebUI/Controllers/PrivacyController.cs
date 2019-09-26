using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Zoro.WebUI.Controllers
{
    public class PrivacyController : SurfaceController
    {
        public PrivacyController()
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleCookieConsent([Bind(Prefix = "cookieConsentModel")]CookiePolicy model, bool? acceptAll = null)
        {
            if (acceptAll.HasValue)
            {
                if (acceptAll.Value == false)
                    CookiePolicy.DoNotAccept();
                else
                {
                    model = new CookiePolicy
                    {
                        Preferences = acceptAll.Value,
                        Marketing = acceptAll.Value,
                        Statistics = acceptAll.Value
                    };

                    model.Save();
                }
            }
            else
            {
                model.Save();
            }

            return RedirectToCurrentUmbracoUrl();
        }

        [HttpGet]
        public ActionResult ExportData(Guid id)
        {
            var member = Services.MemberService.GetByKey(id);
            return Json(member, JsonRequestBehavior.AllowGet);
        }
    }
}
