using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Web.Routing;

namespace UmbracoBootstrap.Web.App_Start
{
    public class RegisterContentFinder : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            // Insert my finder before ContentFinderByNiceUrl
            //ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByNiceUrl, MyCustomContentFinder>();
        }
    }
}
