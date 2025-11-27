namespace MeetingApplication.Interfaces.Integrations
{
    public interface ITeamsService
    {
        Task<string?> GetTranscriptAsync(Guid meetingId, CancellationToken ct = default); // returns storage path or text
        Task<string?> CreateMeetingRecordingLinkAsync(Guid meetingId, CancellationToken ct = default);
    }
}
