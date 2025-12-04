using MassTransit.EntityFrameworkCoreIntegration;
using MeetingApplication.Integrations;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class TeamsOutboxHandler : IOutboxHandler
    {
        private readonly ITeamsService _teams;
        private readonly IStorageService _storage;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public TeamsOutboxHandler(ITeamsService teams, IStorageService storage) { _teams = teams; _storage = storage; }

        public async Task HandleAsync(OutboxMessage message, CancellationToken ct)
        {
            var payload = JsonSerializer.Deserialize<JsonElement>(message.Body);
            if (message.MessageType == "Teams:FetchTranscript")
            {
                var meetingId = payload.GetProperty("meetingId").GetGuid();
                var url = await _teams.FetchTranscriptUrlAsync(meetingId);
                var bytes = await _teams.DownloadTranscriptAsync(url);
                var path = await _storage.SaveFileAsync(bytes, $"transcript_{meetingId}_{DateTime.UtcNow:yyyyMMddHHmm}.txt", "text/plain", ct);

                // optionally enqueue AI job via outbox: "Integration:AI.GenerateMoMFromTranscript"
            }
        }
    }

}
