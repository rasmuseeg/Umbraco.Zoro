namespace Zoro.Domain.Notifications.MailMessages
{
    public interface IEmailButton
    {
        string Url { get; set; }
        string Label { get; set; }
    }
}
