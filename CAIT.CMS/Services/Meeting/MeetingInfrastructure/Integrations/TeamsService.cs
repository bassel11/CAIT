using MeetingApplication.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class TeamsService : ITeamsService
    {
        public Task<string> FetchTranscriptUrlAsync(Guid meetingId)
        {
            return Task.FromResult($"https://fake-teams/{meetingId}/transcript.vtt");
        }

        public Task<byte[]> DownloadTranscriptAsync(string url)
        {
            return Task.FromResult(System.Text.Encoding.UTF8.GetBytes("Fake transcript content"));
        }
    }
}
