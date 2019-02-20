using HtmlAgilityPack;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;

namespace Zoro.Application.Helpers
{
    public class MailMessageHelper
    {

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
                    _notificationsEmailAddress = UmbracoConfig.For.UmbracoSettings().Content.NotificationEmailAddress;
                return _notificationsEmailAddress;
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
    }
}
