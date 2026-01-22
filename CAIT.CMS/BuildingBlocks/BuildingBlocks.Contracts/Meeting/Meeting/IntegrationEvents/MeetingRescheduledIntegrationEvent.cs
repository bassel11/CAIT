namespace BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents
{
    public record MeetingRescheduledIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public DateTime NewStartDate { get; init; }
        public DateTime NewEndDate { get; init; }
        public string? OutlookEventId { get; init; } // لتحديث التقويم
    }
}
