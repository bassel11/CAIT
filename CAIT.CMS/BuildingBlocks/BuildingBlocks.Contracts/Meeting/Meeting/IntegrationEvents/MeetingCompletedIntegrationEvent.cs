namespace BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents
{
    public record MeetingCompletedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public DateTime CompletedAt { get; init; }
    }
}
