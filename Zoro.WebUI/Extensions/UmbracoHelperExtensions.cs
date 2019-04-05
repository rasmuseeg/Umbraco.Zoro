using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Umbraco.Web
{
    public static class UmbracoHelperExtensions
    {
        #region Search Extensions
        /// <summary>
        /// Splits a string on whitespace, except where enclosed in quotes
        /// </summary>
        public static IEnumerable<string> Tokenize(this UmbracoHelper helper, string s)
        {
            Regex regex = new Regex("(?<=\")\\w[\\w\\s]*(?=\")|\\w+|\"[\\w\\s]*\"");
            var matches = regex.Matches(s)
                .Cast<Match>()
                .Select(p => p.Value)
                .ToList();

            return matches;
        }

        /// <summary>
        /// Highlights all occurances of the search terms in a body of text
        /// </summary>
        public static IHtmlString Highlight(this UmbracoHelper helper, IHtmlString s, IEnumerable<string> terms)
        {
            return new HtmlString(helper.Highlight(s.ToString(), terms));
        }

        /// <summary>
        /// Highlights all occurances of the search terms in a body of text
        /// </summary>
        public static string Highlight(this UmbracoHelper helper, string s, IEnumerable<string> terms)
        {
            s = HttpUtility.HtmlDecode(s);

            foreach (var searchTerm in terms)
            {
                s = Regex.Replace(s,
                    Regex.Escape(searchTerm),
                    "<span class=\"highlight\">$0</span>",
                     RegexOptions.IgnoreCase);
            }

            return s;
        }

        public static string Truncate(this UmbracoHelper helper, string s, int maxLength, bool? ellipsis = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Truncates a string on word breaks
        /// </summary>
        public static string Truncate(this UmbracoHelper helper, IHtmlString s, int maxLength)
        {
            return helper.TruncateWordBreak(s.ToString(), maxLength);
        }

        /// <summary>
        /// Truncates a string on word breaks
        /// </summary>
        public static string TruncateWordBreak(this UmbracoHelper helper, string s, int maxLength)
        {
            string truncated = helper.Truncate(s, maxLength, true).ToString();
            if (truncated.EndsWith("&hellip;"))
            {
                int lastSpaceIndex = truncated.LastIndexOf(' ');
                if (lastSpaceIndex > 0)
                {
                    truncated = truncated.Substring(0, lastSpaceIndex) + "&hellip;";
                }
            }

            return truncated;
        }
        #endregion

        #region Umbraco Dictionary
        public static IHtmlString GetDictionaryValue(this UmbracoHelper helper, string key, string fallback)
        {
            string value = helper.GetDictionaryValue(key);
            if (!string.IsNullOrWhiteSpace(value))
                return new HtmlString(value);
            return new HtmlString(fallback);
        }

        public static IHtmlString GetDictionaryValue(this UmbracoHelper helper, string key, string fallback, params object[] par)
        {
            return new HtmlString(string.Format(helper.GetDictionaryValue(key, fallback).ToString(), par));
        }
        #endregion

        /// <summary>
        /// Modified QueryString with key to value
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>?key=value</returns>
        public static string ModifyQueryString<T>(this UmbracoHelper helper, string key, T value)
        {
            var request = HttpContext.Current.Request;
            var nameValues = HttpUtility.ParseQueryString(request.QueryString.ToString());
            nameValues.Set(key, value.ToString());
            return "?" + nameValues.ToString();
        }

        /// <summary>
        /// Gets the querystring key value with fallback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="key">The key of the QueryString</param>
        /// <param name="fallback">If empty or null</param>
        /// <returns></returns>
        public static T GetQueryString<T>(this UmbracoHelper helper, string key, T fallback)
        {
            var request = HttpContext.Current.Request;
            var nameValues = HttpUtility.ParseQueryString(request.QueryString.ToString());
            if (nameValues.AllKeys.Contains(key))
            {
                try
                {
                    return (T)Convert.ChangeType(nameValues.Get(key), typeof(T));
                }
                catch (Exception) { }
            }
            return fallback;
        }

        /// <summary>
        /// Gets the querystring key value with fallback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="key">The key of the QueryString</param>
        /// <param name="fallback">If empty or null</param>
        /// <returns></returns>
        public static T GetQueryString<T>(this UmbracoHelper helper, string key)
        {
            var request = HttpContext.Current.Request;
            var nameValues = HttpUtility.ParseQueryString(request.QueryString.ToString());
            if (nameValues.AllKeys.Contains(key))
            {
                try
                {
                    return (T)Convert.ChangeType(nameValues.Get(key), typeof(T));
                }
                catch (Exception) { }
            }
            return default(T);
        }

        public static bool HasQueryString(this UmbracoHelper helper, string key)
        {
            var request = HttpContext.Current.Request;
            var nameValues = HttpUtility.ParseQueryString(request.QueryString.ToString());
            if (nameValues.AllKeys.Contains(key))
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<T> GetPagedResult<T>(this UmbracoHelper helper, IEnumerable<T> items, int currentPage, int pageSize)
        {
            currentPage = currentPage <= 0 ? 1 : currentPage;
            int skips = (currentPage - 1) * pageSize;
            return items.Skip(skips).Take(pageSize);
        }

        //public static IEnumerable<T> GetPagedResult<T>(this UmbracoHelper helper, IEnumerable<T> items, int currentPage, int pageSize, out PaginationModel pagi)
        //{
        //    pagi = new PaginationModel(items.Count(), currentPage, pageSize);
        //    return items.Skip(pagi.Skips).Take(pagi.PageSize);
        //}
    }
}