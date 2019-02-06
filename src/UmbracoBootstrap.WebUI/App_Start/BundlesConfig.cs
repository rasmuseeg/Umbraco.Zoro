using ClientDependency.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBootstrap.Web.App_Start
{
    public class BundlesConfig
    {
        public static void CreateBundles()
        {
            BundleManager.CreateJsBundle("JQuery",
                new JavascriptFile("~/assets/lib/jquery/dist/jquery.slim.min.js")
            );

            BundleManager.CreateJsBundle("globalize",
                new JavascriptFile("~/assets/lib/cldrjs/dist/cldr.js"),
                new JavascriptFile("~/assets/lib/cldrjs/dist/cldr/event.js"),
                new JavascriptFile("~/assets/lib/cldrjs/dist/cldr/supplemental.js"),
                new JavascriptFile("~/assets/lib/globalize/dist/globalize.js"),
                new JavascriptFile("~/assets/lib/globalize/dist/globalize/number.js"),
                new JavascriptFile("~/assets/lib/globalize/dist/globalize/date.js"),
                new JavascriptFile("~/assets/lib/globalize/dist/globalize/currency.js"),
                new JavascriptFile("~/assets/js/validation/jquery.cldr.js")
            );

            BundleManager.CreateJsBundle("jquery.validation",
               new JavascriptFile("~/assets/lib/jquery-validation/dist/jquery.validate.js"),
               new JavascriptFile("~/assets/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js"),
               new JavascriptFile("~/assets/js/validation/jquery.validation.bootstrap.js"),
               new JavascriptFile("~/assets/js/validation/jquery.validation.custom.js"),
               new JavascriptFile("~/assets/js/validation/jquery.validation.conditional.js")
            );

            BundleManager.CreateJsBundle("Bootstrap",
                new JavascriptFile("https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js"),
                new JavascriptFile("~/assets/lib/bootstrap/dist/js/bootstrap.min.js")
            );
        }
    }
}
