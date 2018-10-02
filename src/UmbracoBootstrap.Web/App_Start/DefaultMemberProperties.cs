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
                string securityKeyAlias = OurConstants.Member.SecurityKey;
                if (!entity.PropertyTypeExists(securityKeyAlias))
                {
                    var propType = new PropertyType(Constants.PropertyEditors.NoEditAlias, 
                        DataTypeDatabaseType.Nvarchar,
                        securityKeyAlias)
                    {
                        Name = OurConstants.Member.SecurityKeyLabel,
                        Description = ""
                    };
                    entity.AddPropertyType(propType);
                }

                string securityKeyTimestampAlias = OurConstants.Member.SecurityKeyTimestamp;
                if (!entity.PropertyTypeExists(securityKeyTimestampAlias))
                {
                    var propType = new PropertyType(Constants.PropertyEditors.NoEditAlias, 
                        DataTypeDatabaseType.Date,
                        securityKeyTimestampAlias)
                    {
                        Name = OurConstants.Member.SecurityKeyTimestampLabel,
                        Description = ""
                    };
                    entity.AddPropertyType(propType);
                }
            }
        }
    }
}
