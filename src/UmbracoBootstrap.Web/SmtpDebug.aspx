<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Net.Mail" %>
<%@ Import Namespace="System.Net.Sockets" %>
<%@ Import Namespace="System.Net.Security" %>
<%@ Import Namespace="System.Net.Configuration" %>
<%@ Import Namespace="Umbraco.Core.Configuration" %>

<%@ Page Language="C#" %>

<script language="c#" runat="server">
    void SendEmail_OnClick(Object sender, EventArgs e)
    {
        var config = UmbracoConfig.For.UmbracoSettings();
        try
        {
            using (var client = new System.Net.Mail.SmtpClient())
            {
                var message = new MailMessage(config.Content.NotificationEmailAddress, this.to.Text);
                message.Subject = "Test";
                message.Body = "If you read this, you smtp settings and server is configured correctly.";
                client.Send(message);
            }
            EmailMessage.Text = "Email was succesfully sent.";
        }
        catch (Exception ex)
        {
            EmailMessage.Text = "[ERROR] Unable to sent email\n\t\t" + ex.ToString();
        }
    }
    void EchoServer_OnClick(Object sender, EventArgs e)
    {
        var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        string username = smtpSection.Network.UserName;
        var server = smtpSection.Network.Host;
        var port = smtpSection.Network.Port;
        try
        {
            using (var client = new TcpClient())
            {
                client.Connect(server, port);
                // As GMail requires SSL we should use SslStream
                // If your SMTP server doesn't support SSL you can
                // work directly with the underlying stream
                if (smtpSection.Network.EnableSsl)
                {
                    this.log.Text += "<br />SSL is enabled.";
                    using (var stream = client.GetStream())
                    using (var sslStream = new SslStream(stream))
                    {
                        sslStream.AuthenticateAsClient(server);
                        using (var writer = new StreamWriter(sslStream))
                        using (var reader = new StreamReader(sslStream))
                        {
                            writer.WriteLine("EHLO " + server);
                            writer.Flush();
                            this.log.Text += "<br />Response: " + reader.ReadLine();
                            Console.WriteLine(reader.ReadLine());
                            // GMail responds with: 220 mx.google.com ESMTP
                        }
                    }
                }
                else
                {
                    this.log.Text += "<br />SSL is disabled.";
                    using (var stream = client.GetStream())
                    using (var writer = new StreamWriter(stream))
                    using (var reader = new StreamReader(stream))
                    {
                        writer.WriteLine("EHLO " + server);
                        writer.Flush();
                        this.log.Text += "<br />Response: " + reader.ReadLine();
                        Console.WriteLine(reader.ReadLine());
                        // GMail responds with: 220 mx.google.com ESMTP
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
</script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SMPT server test</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            E-mail:<br />
            <asp:TextBox ID="to" runat="server" placeholder="Type you e-mail here.."></asp:TextBox>
            <asp:Button ID="SendEmail" OnClick="SendEmail_OnClick" Text="Send" runat="server" />
            <asp:Literal ID="EmailMessage" runat="server" />
            <br />
            <br />
            Test the server response here:
            <asp:Button ID="EchoServer" OnClick="EchoServer_OnClick" Text="ECHO" runat="server" /><br />
            <small>If the server response with 220, we're good.</small>
            <asp:Literal ID="log" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>