using MeetingApplication.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class OutlookService : IOutlookService
    {
        public Task AttachMoMAsync(Guid meetingId, string fileUrl, CancellationToken ct)
        {
            // TODO: integrate Microsoft Graph
            Console.WriteLine($"[Outlook] Attach MoM to meeting {meetingId} at {fileUrl}");
            return Task.CompletedTask;
        }

        public Task UpdateMeetingBodyAsync(Guid meetingId, string bodyHtml, CancellationToken ct)
        {
            Console.WriteLine($"[Outlook] Update meeting body for {meetingId}");
            return Task.CompletedTask;
        }
    }
}
