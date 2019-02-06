using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBootstrap.Web
{
    public static class OurConstants
    {
        public static class Member
        {
            public const string SecurityKeyTimestamp = "umbracoMemberSecurityKeyTimestamp";
            public const string SecurityKeyTimestampLabel = "Security Key Timestamp";
            public const string SecurityKey = "umbracoMemberSecurityKey";
            public const string SecurityKeyLabel = "Security Key";
        }

        public static class MemberGroups
        {
            public const string Default = "Member";
        }

        public static class Dictionary
        {
            public const string RegisterMemberExistError = "RegisterMemberExistError";
        }
    }
}
