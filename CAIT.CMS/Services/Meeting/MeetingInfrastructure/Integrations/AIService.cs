using MeetingApplication.Interfaces.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class AIService : IAIService
    {
        public Task<string> GenerateMoMDraftFromNotesAsync(IEnumerable<string> notes, Guid meetingId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateMoMDraftFromTranscriptAsync(string transcript, Guid meetingId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
