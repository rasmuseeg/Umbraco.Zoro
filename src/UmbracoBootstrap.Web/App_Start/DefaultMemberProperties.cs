using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace UmbracoBootstrap.Web.App_Start
{
    public class DefaultMemberSecurityProperties : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MemberTypeService.Saving += MemberTypeService_Saving;
        }

        private void MemberTypeService_Saving(IMemberTypeService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IMemberType> e)
        {
            foreach(var entity in e.SavedEntities)
            {
                if (!entity.PropertyTypeExists("umbracoMemberSecurityKey"))
                {
                    var propType = new PropertyType("Umbraco.NoEdit", DataTypeDatabaseType.Nvarchar, "umbracoMemberSecurityKey")
                    {
                        Name = "Security Key",
                        Description = ""
                    };
                    entity.AddPropertyType(propType);
                }

                if (!entity.PropertyTypeExists("umbracoMemberSecurityKeyTimestamp"))
                {
                    var propType = new PropertyType("Umbraco.NoEdit", DataTypeDatabaseType.Date, "umbracoMemberSecurityKeyTimestamp")
                    {
                        Name = "Security Key Timestamp",
                        Description = ""
                    };
                    entity.AddPropertyType(propType);
                }
            }
        }
    }
}
