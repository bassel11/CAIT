namespace BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents
{
    public record MeetingScheduledIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public Guid CommitteeId { get; init; }
        public string Title { get; init; } = default!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public List<Guid> AttendeeIds { get; init; } = new(); // لإرسال الدعوات
    }
}
