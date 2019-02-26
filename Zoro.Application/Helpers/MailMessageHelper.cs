using System.Net.Mail;
using System.Threading.Tasks;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;

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
                {
                    var settingsSection = new Configs().GetConfig<IUmbracoSettingsSection>();
                    _notificationsEmailAddress = settingsSection.Content.NotificationEmailAddress;
                }

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
