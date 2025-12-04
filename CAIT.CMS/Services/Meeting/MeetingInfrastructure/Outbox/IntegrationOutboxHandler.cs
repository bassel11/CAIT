using MassTransit.EntityFrameworkCoreIntegration;
using MeetingApplication.Integrations;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class IntegrationOutboxHandler : IOutboxHandler
    {
        private readonly IMessageBus _bus;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        public IntegrationOutboxHandler(IMessageBus bus) => _bus = bus;

        public async Task HandleAsync(OutboxMessage message, CancellationToken ct)
        {
            // type example: "Integration:MoM.Published"
            var routingKey = message.MessageType.Replace("Integration:", "").ToLower(); // e.g. "mom.published"
            var payload = JsonSerializer.Deserialize<JsonElement>(message.Body);
            // publish to message bus
            await _bus.PublishAsync(routingKey, payload, ct);
        }
    }

}
