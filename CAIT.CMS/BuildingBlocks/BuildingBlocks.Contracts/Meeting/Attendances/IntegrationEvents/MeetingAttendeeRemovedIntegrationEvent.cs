namespace BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents
{
    public record MeetingAttendeeRemovedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public Guid MemberId { get; init; }
    }
}
