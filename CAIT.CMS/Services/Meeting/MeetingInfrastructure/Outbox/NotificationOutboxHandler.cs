using MeetingApplication.Integrations;
using MeetingCore.Entities;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class NotificationOutboxHandler : IOutboxHandler
    {
        private readonly INotificationService _ns;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public NotificationOutboxHandler(INotificationService ns) => _ns = ns;

        public async Task HandleAsync(OutboxMessage message, CancellationToken ct)
        {
            var payload = JsonSerializer.Deserialize<JsonElement>(message.Payload);
            var type = message.Type;
            if (type == "Notification:MoMPublished" || type == "Notification:Email")
            {
                var to = payload.GetProperty("to").GetString();
                var subject = payload.GetProperty("subject").GetString() ?? "Notification";
                var body = payload.GetProperty("body").GetString() ?? "";
                if (!string.IsNullOrEmpty(to))
                    await _ns.SendEmailAsync(to, subject, body);
            }
            else if (type == "Notification:Push")
            {
                var userId = payload.GetProperty("userId").GetGuid();
                var title = payload.GetProperty("title").GetString()!;
                var body = payload.GetProperty("body").GetString()!;
                await _ns.SendPushAsync(userId, title, body);
            }
        }
    }

}
