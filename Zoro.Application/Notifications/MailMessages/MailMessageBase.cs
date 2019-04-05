using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Xml.Linq;
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

        public virtual MailMessage GetMailMessage(string to)
        {
            XDocument doc = ReadAsHtmlDocument();
            string subject = HttpUtility.HtmlDecode(doc.Element("title").Value);
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
        public virtual void Send(string to)
        {
            var message = GetMailMessage(to);
            MailMessageHelper.Current.SendMail(message);
        }

        /// <summary>
        /// Reads the view as string
        /// </summary>
        /// <param name="controllerContext">The current controller context.</param>
        /// <returns>The view as string </returns>
        public virtual string ReadAsString()
        {
            return ViewHelper.RenderView(ViewPath, this);
        }

        public virtual XDocument ReadAsHtmlDocument()
        {
            return XDocument.Parse(ViewHelper.RenderView(ViewPath, this));
        }
    }
}
