using System;
using System.Configuration;

namespace Zoro.WebUI
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
