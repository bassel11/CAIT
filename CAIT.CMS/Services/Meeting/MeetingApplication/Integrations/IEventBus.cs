namespace MeetingApplication.Integrations
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class;
    }
}
