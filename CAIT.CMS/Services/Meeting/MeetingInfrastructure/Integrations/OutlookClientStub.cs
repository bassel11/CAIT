namespace MeetingInfrastructure.Integrations
{
    public class OutlookClientStub
    {
        public Task AttachFileToMeetingAsync(string meetingId, string storagePath)
        {
            // Implement real graph/outlook call here
            return Task.CompletedTask;
        }
    }
}
