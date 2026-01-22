namespace BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents
{
    public record MeetingAttendeeAddedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public Guid MemberId { get; init; }
        public string Role { get; init; } = default!;
    }
}
