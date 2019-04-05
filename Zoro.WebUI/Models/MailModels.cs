using System;
using Umbraco.Core.Models;
using ContentModels = Zoro.WebUI.Models.PublishedContent;

namespace Zoro.WebUI.Models
{
    public class RecoverAccountConfirmationMailModel
    {
        public ContentModels.Member Member { get; set; }

        public string Permalink { get; set; }
    }

    public class ApproveEmailMailModel
    {
        public IPublishedContent Member { get; set; }

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
