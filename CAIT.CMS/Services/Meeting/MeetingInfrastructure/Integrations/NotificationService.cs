using MeetingApplication.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class NotificationService : INotificationService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"[Email] To: {to} | {subject}");
            return Task.CompletedTask;
        }

        public Task SendPushAsync(Guid userId, string title, string body)
        {
            Console.WriteLine($"[Push] User {userId} | {title}");
            return Task.CompletedTask;
        }
    }
}
