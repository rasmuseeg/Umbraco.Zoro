using System.Collections.Generic;
using Umbraco.Web;

namespace System
{
    public static class DateTimeExtensions
    {
        #region Momentjs
        private static Dictionary<string, string> relativeTime = new Dictionary<string, string> {
            { "future", "om {0}" },
            { "past", "{0} siden" },
            { "s", "sekunder" },
            { "m", "ét minut" },
            { "mm", "{0} minutter" },
            { "h", "en time" },
            { "hh", "{0} timer" },
            { "d", "en dag" },
            { "dd", "{0} dage" },
            { "M", "en måned" },
            { "MM", "{0} måneder" },
            { "y", "et år" },
            { "yy", "{0} år" }
        };

        /// <summary>
        /// Time from now
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Pretty formatted date like: </returns>
        public static string FromNow(this DateTime d, bool surfix = true)
        {
            // Determine past or future
            if (surfix == false)
                return string.Format("{0}", GetDurationFormat(d));

            string format = d > DateTime.Now ? relativeTime["future"] : relativeTime["past"];
            return string.Format(format, GetDurationFormat(d));
        }

        private static string GetDurationFormat(DateTime d)
        {
            // 1.
            // Get time span elapsed since the date. 
            TimeSpan duration = DateTime.Now.Subtract(d).Duration();

            // 2.
            // Get total number of seconds elapsed.
            int secDifference = (int)duration.TotalSeconds;

            // Greater than one minute.
            if (secDifference <= 45)
                return relativeTime["s"];

            // From 45 seconds to 90 seconds
            else if (secDifference <= 90)
                return relativeTime["m"];

            // From 90 seconds to 45 minutes
            else if (secDifference <= 2700)
                return string.Format(relativeTime["mm"], Math.Floor((double)secDifference / 60));

            // From 45 to 90 minutes ago
            else if (secDifference <= 5400)
                return relativeTime["h"];

            // Get the total number of days
            int hoursDifference = (int)duration.TotalHours;

            // From 90 minutes to 22 hours 
            if (hoursDifference <= 22)
                return string.Format(relativeTime["hh"], hoursDifference);
            // From 22 to 36 hours
            else if (hoursDifference <= 36)
                return relativeTime["d"];

            // Get total number of days elapsed.
            int dayDifference = (int)duration.TotalDays;

            // From 36 hours to 25 days
            if (dayDifference <= 25)
                return string.Format(relativeTime["dd"], dayDifference);

            // From 25 to 45 days
            else if (dayDifference <= 45)
                return relativeTime["m"];

            // From 45 to 345 days
            else if (dayDifference <= 345)
                return string.Format(relativeTime["mm"], Math.Floor((double)dayDifference / 345));

            // From 345 to 545 days (1.5 years)
            else if (dayDifference <= 545)
                return relativeTime["y"];

            else
                return string.Format(relativeTime["yy"], Math.Floor((double)dayDifference / 365));
        }
        #endregion
    }
}