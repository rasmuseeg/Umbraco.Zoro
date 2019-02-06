using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentModels = UmbracoBootstrap.Web.PublishedContentModels;

namespace UmbracoBootstrap.Web.Models
{
    public class RecoverAccountConfirmationMailModel
    {
        public ContentModels.Member Member { get; set; }

        public string Permalink { get; set; }
    }

    public class ApproveEmailMailModel
    {
        public ContentModels.Member Member { get; set; }

        /// <summary>
        /// Permanent link to where member can confirm their email.
        /// </summary>
        public Uri Permalink { get; set; }
    }

    public class DeleteAccountMailModel
    {

    }

    public class CallToAction
    {
        public string Url { get; set; }

        public string Label { get; set; }
    }
}
