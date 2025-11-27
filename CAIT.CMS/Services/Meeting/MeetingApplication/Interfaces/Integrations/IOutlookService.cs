namespace MeetingApplication.Interfaces.Integrations
{
    public interface IOutlookService
    {
        Task SyncMeetingAttachmentAsync(Guid meetingId, string attachmentUrl, CancellationToken ct = default);
        Task UpdateMeetingWithMinutesLinkAsync(Guid meetingId, string link, CancellationToken ct = default);
    }
}
