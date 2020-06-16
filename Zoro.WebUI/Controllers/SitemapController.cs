 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using System.Threading.Tasks;
 using System.Web.Mvc;
 using System.Xml.Linq;
 using System;
 using DevTrends.MvcDonutCaching;
 using Umbraco.Core.Models;
 using Umbraco.Web.Models;
 using Umbraco.Web.Mvc;
 using Umbraco.Web;

 namespace $namespace$.Controllers
 {
    public class SitemapController : RenderMvcController
    {
        [DonutOutputCache (Duration = 3600, VaryByCustom = "Url")]
        public override ActionResult Index (RenderModel model)
        {
            string domain = Request.Url.GetLeftPart (UriPartial.Authority);

            XDocument doc = new XDocument (new XDeclaration ("1.0", "utf-8", "yes"));
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement channel = new XElement (ns + "urlset");

            channel.Add (GetItemAndChildren (ns, CurrentPage.Site (), 1.0m));

            doc.Add (channel);

            // Add headers
            Response.AddHeader ("Content-Type", "application/xml, text/xml; charset=utf-8");
            Response.AddHeader ("Content-Disposition", "filename=sitemap.xml");

            // write the xml document to response
            var sb = new StringBuilder ();
            sb.AppendLine ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append (doc.ToString ());

            return Content (sb.ToString (), "application/xml, text/xml; charset=utf-8");
        }

        private IEnumerable<XElement> GetItemAndChildren (XNamespace ns, IPublishedContent item, decimal priority)
        {
            var items = new List<XElement> ();

            XElement xmlItem = new XElement (ns + "url",
                new XElement (ns + "loc", item.UrlAbsolute ()),
                new XElement (ns + "lastmod", (item.UpdateDate).ToString ("yyyy-MM-dd")),
                new XElement (ns + "changefreq", GetChangeFreq (item)),
                new XElement (ns + "priority", item.GetPropertyValue<decimal?> ("sitemapPriority", priority))
            );
            items.Add (xmlItem);

            var children = item.Children.Where (x => x.IsVisible ()
                    && x.TemplateId > 0
                    && !x.GetPropertyValue<bool> ("hideFromSitemap", false));

            if (children.Count () > 0)
            {
                foreach (var child in children)
                {
                    items.AddRange (GetItemAndChildren (ns, child, Math.Max (priority - 0.1m, 0.1m)));
                }
            }

            return items;
        }

        private string GetChangeFreq (IPublishedContent item)
        {
            string changefreq = item.GetPropertyValue<string> ("sitemapChangeFreq", null);
            if (string.IsNullOrEmpty (changefreq))
            {
                switch (item.DocumentTypeAlias)
                {
                    // Add custom logic here for each documenttype
                    default : return "monthly";
                }
            }
            else return changefreq;
        }
    }
}