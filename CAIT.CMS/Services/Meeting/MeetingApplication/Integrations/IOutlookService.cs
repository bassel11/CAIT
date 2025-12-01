namespace MeetingApplication.Integrations
{
    public interface IOutlookService
    {
        Task AttachMoMAsync(Guid meetingId, string fileUrl, CancellationToken ct);
        Task UpdateMeetingBodyAsync(Guid meetingId, string bodyHtml, CancellationToken ct);
    }
}
