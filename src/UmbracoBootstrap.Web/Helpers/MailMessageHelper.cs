using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;

namespace UmbracoBootstrap.Web.Helpers
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

        public void SendMail(string subject, string bodyText, string to, string from = null)
        {
            using (var message = new MailMessage())
            {
                message.Body = bodyText;
                message.To.Add(to);
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;
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
                LogHelper.Warn<MailMessageHelper>("Unable to find view: {0}", () => viewPath);
                culture = CultureInfo.InvariantCulture.Name;
                viewPath = "~/Views/MailMessages/" + culture + "/" + templateKey + ".cshtml";
            }

            string html = ViewHelper.RenderView(viewPath, model, controllerContext);

            // Parse title
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string subject = doc.DocumentNode.SelectSingleNode("//title").InnerText;

            var mailMessage = new MailMessage(NotificationsEmailAddress, to)
            {
                Subject = subject,
                Body = html,
                IsBodyHtml = true
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

        //public string ParseTemplate<T>(string viewPath, T model)
        //{
        //    string templateKey = viewPath;
        //    string relativePath = viewPath;
        //    if (viewPath.StartsWith("~/"))
        //        relativePath = relativePath.Substring(2);

        //    Type modelType = model.GetType();

        //    string templateAbsolutePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, relativePath);

        //    if (Engine.Razor.IsTemplateCached(templateKey, modelType) == false)
        //    {
        //        string templateSource = System.IO.File.ReadAllText(templateAbsolutePath);
        //        return Engine.Razor.RunCompile(templateSource, templateKey, modelType, model);
        //    }
        //    else
        //    {
        //        return Engine.Razor.Run(templateKey, modelType, model);
        //    }
        //}

        //public string GetAbsolutePath(string relativePath, string basePath)
        //{
        //    if (relativePath == null)
        //        return null;
        //    if (basePath == null)
        //        basePath = Path.GetFullPath("."); // quick way of getting current working directory
        //    else
        //        basePath = GetAbsolutePath(basePath, null); // to be REALLY sure ;)
        //    string path;
        //    // specific for windows paths starting on \ - they need the drive added to them.
        //    // I constructed this piece like this for possible Mono support.
        //    if (!Path.IsPathRooted(relativePath) || "\\".Equals(Path.GetPathRoot(relativePath)))
        //    {
        //        if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()))
        //            path = Path.Combine(Path.GetPathRoot(basePath), relativePath.TrimStart(Path.DirectorySeparatorChar));
        //        else
        //            path = Path.Combine(basePath, relativePath);
        //    }
        //    else
        //        path = relativePath;
        //    // resolves any internal "..\" to get the true full path.
        //    return Path.GetFullPath(path);
        //}
    }
}
