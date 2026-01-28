namespace MeetingApplication.Interfaces.Scheduling
{
    public interface IMeetingSchedulerGateway
    {
        Task ScheduleMeetingRemindersAsync(Guid meetingId, string title, DateTime startDate, CancellationToken ct);
        Task CancelMeetingRemindersAsync(Guid meetingId, CancellationToken ct);
    }
}
