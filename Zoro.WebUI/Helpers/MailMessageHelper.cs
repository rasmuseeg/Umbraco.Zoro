using HtmlAgilityPack;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;

namespace Zoro.WebUI.Helpers
{
    public class MailMessageHelper
    {
        //private static ILogger Logger = 

        private static MailMessageHelper _current;
        public static MailMessageHelper Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new MailMessageHelper();
                }
                return _current;
            }
        }

        private string _notificationsEmailAddress;
        public string NotificationsEmailAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_notificationsEmailAddress))
                {
                    _notificationsEmailAddress = Umbraco.Web.Composing.Current.Configs.GetConfig<IUmbracoSettingsSection>().Content.NotificationEmailAddress;
                }
                return _notificationsEmailAddress;
            }
        }

        public void SendMail(string subject, string bodyText, string to, string from = null)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(to);
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.Body = bodyText;
                message.SubjectEncoding = Encoding.UTF8;
                message.Subject = subject;

                SendMail(message);
            }
        }

        public void SendMail(MailMessage message)
        {
            using (var client = new SmtpClient())
                client.Send(message);
        }

        public async Task SendMailAsync(MailMessage message)
        {
            using (var client = new SmtpClient())
                await client.SendMailAsync(message);
        }

        public MailMessage MailMessageFromView(string to, string templateKey, object model, System.Web.Mvc.ControllerContext controllerContext)
        {
            string culture = CultureInfo.CurrentCulture.Name;
            string viewPath = "~/Views/MailMessages/" + culture + "/" + templateKey + ".cshtml";

            string absoluteViewPath = HostingEnvironment.MapPath(viewPath);
            var file = new FileInfo(absoluteViewPath);
            if (!file.Exists) {
                //TODO: Logger.Warn<MailMessageHelper>("Unable to find view: {0}", () => viewPath);
                culture = CultureInfo.InvariantCulture.Name;
                viewPath = "~/Views/MailMessages/" + culture + "/" + templateKey + ".cshtml";
            }

            string html = ViewHelper.RenderView(viewPath, model, controllerContext);

            // Parse title
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string subject = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//title").InnerText);

            var mailMessage = new MailMessage(NotificationsEmailAddress, to)
            {
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = html,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
            };
            return mailMessage;
        }

        public void SendMailMessage(string to, string templateKey, object model, System.Web.Mvc.ControllerContext controllerContext)
        {
            using(var mailMessage = MailMessageFromView(to, templateKey, model, controllerContext))
            {
                SendMail(mailMessage);
            }
        }

        public async Task SendMailMessageAsync(string to, string templateKey, object model, System.Web.Mvc.ControllerContext controllerContext)
        {
            using (var mailMessage = MailMessageFromView(to, templateKey, model, controllerContext))
            {
                await SendMailAsync(mailMessage);
            }
        }
    }
}
