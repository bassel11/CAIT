using MassTransit.EntityFrameworkCoreIntegration;

namespace MeetingInfrastructure.Outbox
{
    public interface IOutboxHandler
    {
        Task HandleAsync(OutboxMessage message, CancellationToken ct);
    }

}
