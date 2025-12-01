namespace MeetingApplication.Integrations
{
    public interface IAIService
    {
        Task<string> GenerateMoMDraftFromTranscriptAsync(string transcript, Guid meetingId, CancellationToken ct = default);
        Task<string> GenerateMoMDraftFromNotesAsync(IEnumerable<string> notes, Guid meetingId, CancellationToken ct = default);
    }
}

