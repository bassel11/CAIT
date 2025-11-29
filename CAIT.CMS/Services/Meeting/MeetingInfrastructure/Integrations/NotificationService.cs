using MeetingApplication.Interfaces.Integrations;
using MeetingCore.Entities;

namespace MeetingInfrastructure.Integrations
{
    public class NotificationService : INotificationService
    {
        public Task QueueNotificationAsync(MeetingNotification notification, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
