namespace BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents
{
    public record MeetingAttendeeRSVPedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public Guid MemberId { get; init; }
        public string RSVPStatus { get; init; } = default!; // نستخدم string لسهولة النقل (JSON)
        public DateTime Timestamp { get; init; }
    }
}
