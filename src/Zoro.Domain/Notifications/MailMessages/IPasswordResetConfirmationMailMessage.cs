namespace Zoro.Domain.Notifications.MailMessages
{
    public interface IPasswordResetConfirmationMailMessage
    {
        string FullName { get; }

        string Permalink { get; }
    }
}
