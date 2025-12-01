using MeetingApplication.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class EventBus : IEventBus
    {
        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class
        {
            throw new NotImplementedException();
        }
    }
}
