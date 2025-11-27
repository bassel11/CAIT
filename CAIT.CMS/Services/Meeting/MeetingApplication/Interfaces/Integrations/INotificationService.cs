using MeetingCore.Entities;

namespace MeetingApplication.Interfaces.Integrations
{
    public interface INotificationService
    {
        Task QueueNotificationAsync(MeetingNotification notification, CancellationToken ct = default);
    }
}
