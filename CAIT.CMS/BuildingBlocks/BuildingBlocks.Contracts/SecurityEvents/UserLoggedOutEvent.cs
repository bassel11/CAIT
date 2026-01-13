namespace BuildingBlocks.Contracts.SecurityEvents
{
    public record UserLoggedOutEvent
    {
        public Guid UserId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
