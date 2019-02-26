using Umbraco.Web;

namespace UmbracoBootstrap.Web.PublishedContentModels
{
    public partial class Member
    {
        public string Email
        {
            get { return this.GetPropertyValue<string>("Email"); }
        }

        public string UserName
        {
            get { return this.GetPropertyValue<string>("UserName"); }
        }

        public bool IsApproved
        {
            get { return this.UmbracoMemberApproved; }
        }
    }
}
