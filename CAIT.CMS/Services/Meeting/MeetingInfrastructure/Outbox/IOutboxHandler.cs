using MeetingCore.Entities;

namespace MeetingInfrastructure.Outbox
{
    public interface IOutboxHandler
    {
        Task HandleAsync(OutboxMessage message, CancellationToken ct);
    }

}
