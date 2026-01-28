namespace BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents
{
    public record MeetingCompletedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public DateTime CompletedAt { get; init; }
    }
}
