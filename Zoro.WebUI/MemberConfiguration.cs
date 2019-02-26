using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBootstrap.Web
{
    public class MemberConfiguration
    {
        public static TimeSpan _expiryDuration;
        public static TimeSpan SecurityKeyExpiryDuration
        {
           get
            {
                if (_expiryDuration == null)
                {
                    string strExpirationTime = ConfigurationManager.AppSettings.Get("Member.SecurityKeyExpiryDuration");
                    if (string.IsNullOrEmpty(strExpirationTime))
                    {
                        _expiryDuration = new TimeSpan(24, 0, 0);
                    }
                    else
                    {
                        _expiryDuration = TimeSpan.Parse(strExpirationTime);
                    }
                }
                return _expiryDuration;
            }
        }
    }
}
