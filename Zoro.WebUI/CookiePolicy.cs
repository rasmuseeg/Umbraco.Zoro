using Newtonsoft.Json;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zoro.WebUI
{
     public class CookiePolicy
    {
        private static string[] RESERVED_COOKIES = new[] { "ASP.NET_SessionId" };
        public const string COOKIE_NAME = "CookieConsent";

        #region Constructors 
        public CookiePolicy()
        {
        }

        public CookiePolicy(Guid id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }

        internal static void DoNotAccept()
        {
            // Clear cookies
            var cookies = HttpContext.Current.Request.Cookies;
            for (int i = 0; i < cookies.Count; i++)
            {
                string cookieName = cookies[i].Name;
                if (RESERVED_COOKIES.Contains(cookieName))
                {
                    continue;
                }

                var myCookie = new HttpCookie(cookieName) {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                HttpContext.Current.Response.SetCookie(myCookie);
            }

            var saved = GetSavedOrDefault();
            saved.AcceptAll = false;
            saved.Save();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The user must accept nescesary cookies.
        /// </summary>
        [JsonProperty("nescesary", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Nescesary { get; set; }

        /// <summary>
        /// Wether or not user has accepted the use of preferences cookies.
        /// </summary>
        [JsonProperty("preferences", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Preferences { get; set; }

        /// <summary>
        /// Wether or not user has accepted the use of statistic cookies
        /// </summary>
        [JsonProperty("statistics", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Statistics { get; set; }

        /// <summary>
        /// Wether or not user has accepted the use of marketing cookies
        /// </summary>
        [JsonProperty("marketing", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Marketing { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime Timestamp { get; set; }

        [JsonIgnore]
        public bool AcceptAll
        {
            get {
                return (this.Nescesary
                    && this.Preferences
                    && this.Statistics
                    && this.Marketing);
            }
            set {
                Nescesary = value;
                Preferences = value;
                Statistics = value;
                Marketing = value;
            }
        }

        /// <summary>
        /// Check if any was accepted.
        /// </summary>
        /// <returns></returns>
        public bool AcceptAny()
        {
            return (this.Nescesary
                    || this.Preferences
                    || this.Statistics
                    || this.Marketing);
        }
        #endregion

        /// <summary>
        /// Stamps populates timestamp and id
        /// </summary>
        public void Stamp()
        {
            Timestamp = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Get saved cookie consent if version matches
        /// </summary>
        /// <param name="version">Only if version matches</param>
        /// <returns></returns>
        public static CookiePolicy GetSaved(DateTime? timestamp = null)
        {
            string savedValue = HttpContext.Current.Request.Cookies[COOKIE_NAME]?.Value
                ?? HttpContext.Current.Session[COOKIE_NAME] as string;
            if (savedValue != null)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(savedValue);
                    string json = Encoding.UTF8.GetString(bytes);
                    var saved = JsonConvert.DeserializeObject<CookiePolicy>(json);
                    if (timestamp.HasValue)
                    {
                        if (saved.Timestamp >= timestamp)
                            return saved;

                        return null;
                    }
                    return saved;
                }
                catch (JsonSerializationException ex)
                {
                    // Just ignore saved value
                }
            }
            return null;
        }

        /// <summary>
        /// Gets saved or Default
        /// </summary>
        /// <returns></returns>
        public static CookiePolicy GetSavedOrDefault(DateTime? timestamp = null)
        {
            var consent = GetSaved(timestamp);
            if (consent != null)
                return consent;

            return new CookiePolicy();
        }

        public static bool Exist(DateTime? timestamp = null)
        {
            if (GetSaved(timestamp) != null)
                return true;
            return false;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            var response = HttpContext.Current.Response;
            var request = HttpContext.Current.Request;

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            string base64 = Convert.ToBase64String(bytes);

            if (AcceptAny())
            {
                var cookie = new HttpCookie(COOKIE_NAME, base64) {
                    Expires = DateTime.Now.AddDays(30),
                    Domain = request.Url.Host,
                    Path = "/",
                    Secure = request.Url.Scheme == "https",
                    HttpOnly = false // Client-side script, should not change this cookie
                };

                var savedConsent = GetSaved();
                if (savedConsent != null)
                {
                    // Renew
                    response.SetCookie(cookie);
                }
                else
                {
                    response.AppendCookie(cookie);
                }
            }
            else
            {
                HttpContext.Current.Session[COOKIE_NAME] = base64;
            }
        }
    }
}
