using MeetingApplication.Interfaces.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class OutlookService : IOutlookService
    {
        public Task SyncMeetingAttachmentAsync(Guid meetingId, string attachmentUrl, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMeetingWithMinutesLinkAsync(Guid meetingId, string link, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
