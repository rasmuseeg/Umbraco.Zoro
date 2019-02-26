using System;
using System.Web;
using System.Web.Mvc;

namespace Zoro.Application.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteUrl(this UrlHelper urlHelper, string virtualPath)
        {
            var context = HttpContext.Current;

            if (virtualPath.StartsWith("~"))
            {
                virtualPath = virtualPath.Substring(1);
            }

            return new Uri(context.Request.Url, virtualPath).AbsoluteUri;
        }
    }
}
