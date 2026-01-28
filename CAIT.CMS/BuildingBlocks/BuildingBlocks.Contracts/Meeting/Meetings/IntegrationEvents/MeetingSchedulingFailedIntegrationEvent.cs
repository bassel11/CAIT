namespace BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents
{
    public record MeetingSchedulingFailedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public string Reason { get; init; } = default!;
        public DateTime FailureTime { get; init; } = DateTime.UtcNow;

        // يمكن إضافة قائمة بالأعضاء المشغولين لاحقاً لتحسين تجربة المستخدم
        // public List<string> BusyAttendees { get; init; } 
    }
}
