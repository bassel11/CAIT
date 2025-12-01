using MeetingApplication.Integrations;
using MeetingCore.Entities;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class OutlookOutboxHandler : IOutboxHandler
    {
        private readonly IOutlookService _outlook;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public OutlookOutboxHandler(IOutlookService outlook) => _outlook = outlook;

        public async Task HandleAsync(OutboxMessage message, CancellationToken ct)
        {
            var doc = JsonSerializer.Deserialize<JsonElement>(message.Payload);
            if (message.Type == "Outlook:AttachMoM")
            {
                var meetingId = doc.GetProperty("meetingId").GetGuid();
                var url = doc.GetProperty("url").GetString()!;
                await _outlook.AttachMoMAsync(meetingId, url, ct);
            }
            else if (message.Type == "Outlook:UpdateBody")
            {
                var meetingId = doc.GetProperty("meetingId").GetGuid();
                var body = doc.GetProperty("bodyHtml").GetString()!;
                await _outlook.UpdateMeetingBodyAsync(meetingId, body, ct);
            }
        }
    }

}
