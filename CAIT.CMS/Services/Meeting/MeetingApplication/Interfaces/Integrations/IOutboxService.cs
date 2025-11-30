using MeetingCore.Entities;

namespace MeetingApplication.Interfaces.Integrations
{
    public interface IOutboxService
    {
        Task EnqueueAsync(OutboxMessage message, CancellationToken ct = default);
    }
}
