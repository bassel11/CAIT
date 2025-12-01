using MeetingApplication.Integrations;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingInfrastructure.Outbox
{
    public class OutboxRouter : IOutboxRouter
    {
        private readonly IServiceProvider _sp;
        public OutboxRouter(IServiceProvider sp) => _sp = sp;

        public IOutboxHandler Resolve(string messageType)
        {
            if (messageType.StartsWith("Integration:")) return _sp.GetRequiredService<IntegrationOutboxHandler>();
            if (messageType.StartsWith("Outlook:")) return _sp.GetRequiredService<OutlookOutboxHandler>();
            if (messageType.StartsWith("Notification:")) return _sp.GetRequiredService<NotificationOutboxHandler>();
            if (messageType.StartsWith("Teams:")) return _sp.GetRequiredService<TeamsOutboxHandler>();
            if (messageType.StartsWith("Audit:")) return _sp.GetRequiredService<AuditOutboxHandler>();

            throw new InvalidOperationException($"Unknown outbox message type: {messageType}");
        }
    }
}
