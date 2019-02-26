using HtmlAgilityPack;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Web;
using Zoro.Application.Helpers;
using Zoro.Domain.Notifications;

namespace Zoro.Application.Notifications.MailMessages
{
    public abstract class MailMessageBase : INotification
    {
        public string Subject { get; set; }

        public string ViewDirectory { get; set; } = "~/Views/Emails";

        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public virtual string ViewPath
        {
            get
            {
                string culture = Culture.Name;
                string viewPath = ViewDirectory + "/" + culture + "/" + TemplateFileName + ".cshtml";
                return viewPath;
            }
        }

        /// <summary>
        /// Name of the cshtml file
        /// </summary>
        public virtual string TemplateFileName
        {
            get
            {
                string name = this.GetType().Name;
                if (!name.EndsWith("MailMessage"))
                {
                    throw new System.Exception($"Derivered MailMessage class \"{name}\" does not end with 'MailMessage'.");
                }

                return name;
            }
        }

        public virtual MailMessage GetMailMessage(string to, System.Web.Mvc.ControllerContext controllerContext = null)
        {
            HtmlDocument doc = ReadAsHtmlDocument();
            string subject = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//title").InnerText);
            string from = MailMessageHelper.Current.NotificationsEmailAddress;

            var mailMessage = new MailMessage(from, to)
            {
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = doc.ToString(),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
            };

            return mailMessage;
        }

        /// <summary>
        /// Send the mail message
        /// </summary>
        /// <param name="to">Receipents mail address</param>
        public virtual void Send(string to, System.Web.Mvc.ControllerContext controllerContext = null)
        {
            var message = GetMailMessage(to, controllerContext);
            MailMessageHelper.Current.SendMail(message);
        }

        /// <summary>
        /// Reads the view as string
        /// </summary>
        /// <param name="controllerContext">The current controller context.</param>
        /// <returns>The view as string </returns>
        public virtual string ReadAsString(System.Web.Mvc.ControllerContext controllerContext = null)
        {
            return ViewHelper.RenderView(ViewPath, this, controllerContext);
        }

        public virtual HtmlDocument ReadAsHtmlDocument(System.Web.Mvc.ControllerContext controllerContext = null)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ViewHelper.RenderView(ViewPath, this, controllerContext));
            return doc;
        }
    }
}
