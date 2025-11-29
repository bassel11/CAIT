using MeetingApplication.Interfaces.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class TeamsService : ITeamsService
    {
        public Task<string?> CreateMeetingRecordingLinkAsync(Guid meetingId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetTranscriptAsync(Guid meetingId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
