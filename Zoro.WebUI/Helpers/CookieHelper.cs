using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

//{stamp:'yPe34YH8yaKBV4dFyhqdIvLlfUOJ9sq77ebghnDKWx4BVYzjeH2AuQ=='%2Cnecessary:true%2Cpreferences:true%2Cstatistics:true%2Cmarketing:true%2Cver:1}

namespace Zoro.WebUI.Helpers
{
    public static class PrivacyHelper
    {
        public const string COOKIE_CONSENT_ALIAS = "CookieConsent";

        public static bool HasCookieConsent
        {
            get
            {
                return GetPrivacyCookie.Nescessary;
            }
        }

        public static HttpPolicyCookie GetPrivacyCookie
        {
            get
            {
                var cookie = GetCookie(COOKIE_CONSENT_ALIAS);
                if (cookie == null)
                    return null;
                var policyCookie = JsonConvert.DeserializeObject<HttpPolicyCookie>(cookie.Value);
                return policyCookie;
            }
        }

        private static HttpCookie GetCookie(string name)
        {
            var cookie = HttpContext.Current.Request.Cookies[COOKIE_CONSENT_ALIAS];
            if (cookie == null)
                return null;

            return cookie;
        }
    }

    public class HttpPolicyCookie
    {
        public bool Nescessary { get; set; }
        public bool Preferences { get; set; }
        public bool Statistics { get; set; }
        public bool Marketing { get; set; }
    }
}
