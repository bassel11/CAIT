namespace MeetingApplication.Integrations
{
    public interface IMessageBus
    {
        Task PublishAsync(string routingKey, object payload, CancellationToken ct = default);
    }
}
