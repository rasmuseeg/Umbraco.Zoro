namespace Zoro.Domain.Notifications.MailMessages
{
    public interface IPasswordChangedMailMessage
    {
        string FullName { get; }
    }
}
