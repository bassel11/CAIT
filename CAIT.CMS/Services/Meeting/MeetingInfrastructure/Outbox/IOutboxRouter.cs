using MeetingInfrastructure.Outbox;

namespace MeetingApplication.Integrations
{
    public interface IOutboxRouter
    {
        IOutboxHandler Resolve(string messageType);
    }
}
