namespace BuildingBlocks.Contracts.SecurityEvents
{
    public record UserPermissionsChangedIntegrationEvent
    {
        public Guid UserId { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

}
