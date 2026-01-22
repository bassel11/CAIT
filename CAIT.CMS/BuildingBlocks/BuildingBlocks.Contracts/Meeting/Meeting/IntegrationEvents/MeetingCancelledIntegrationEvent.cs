namespace BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents
{
    public record MeetingCancelledIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public string Reason { get; init; } = default!;
        public string? OutlookEventId { get; init; }
        public List<Guid> AttendeeIds { get; init; } = new(); // لإرسال إشعار الإلغاء
    }
}
