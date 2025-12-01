namespace MeetingApplication.Integrations
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPushAsync(Guid userId, string title, string body);
    }
}
