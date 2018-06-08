using Newtonsoft.Json;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UmbracoBootstrap.Web
{
    public class CookiePolicy
    {
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
            var cookies = HttpContext.Current.Request.Cookies;
            for (int i = 0; i < cookies.Count; i++)
            {
                var cookieName = cookies[i].Name;

                if (COOKIE_NAME == cookieName)
                {
                    var saved = GetSaved();
                    saved.Preferences = false;
                    saved.Marketing = false;
                    saved.Statistics = false;
                    saved.Save();
                    continue;
                }

                HttpCookie myCookie = new HttpCookie(cookieName)
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                HttpContext.Current.Response.SetCookie(myCookie);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The user must accept nescesary cookies.
        /// </summary>
        [JsonProperty("nescesary", NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public readonly bool Nescesary = true;

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
            get
            {
                return (this.Nescesary
                    && this.Preferences
                    && this.Statistics
                    && this.Marketing);
            }
            set
            {
                Preferences = value;
                Statistics = value;
                Marketing = true;
            }
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
            var cookie = HttpContext.Current.Request.Cookies[COOKIE_NAME];
            if (cookie != null)
            {
                var saved = JsonConvert.DeserializeObject<CookiePolicy>(cookie.Value);
                if (timestamp.HasValue)
                {
                    if (saved.Timestamp >= timestamp)
                        return saved;

                    return null;
                }
                return saved;
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

            var cookie = new HttpCookie(COOKIE_NAME, json)
            {
                Expires = DateTime.Now.AddDays(30),
                Domain = request.Url.Host,
                Path = "/",
                Secure = request.Url.Scheme == "https",
                HttpOnly = false // Visible to client-side script
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
    }
}
