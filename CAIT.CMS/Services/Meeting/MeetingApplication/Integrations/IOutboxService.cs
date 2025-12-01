namespace MeetingApplication.Integrations
{
    public interface IOutboxService
    {
        //Task EnqueueAsync(OutboxMessage message, CancellationToken ct = default);
        Task EnqueueAsync(string type, object payload, CancellationToken ct = default);
    }
}
