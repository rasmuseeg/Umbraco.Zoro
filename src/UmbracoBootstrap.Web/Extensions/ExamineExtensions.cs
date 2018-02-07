using System;
using System.Globalization;

namespace Umbraco.Examine
{
    public static class ExamineExtensions
    {
        private static string format = "yyyyMMddHHmmssfff";

        public static DateTime? ParseExamineDate(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException("s");

            DateTime dt;
            if (DateTime.TryParseExact(s, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;

            return null;
        }

        public static string ToExamineString(this DateTime d)
        {
            if(d == null)
                throw new ArgumentNullException("d");
                
            return d.ToString(format);
        }
    }
}