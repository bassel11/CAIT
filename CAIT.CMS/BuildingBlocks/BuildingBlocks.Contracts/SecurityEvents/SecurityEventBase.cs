namespace BuildingBlocks.Contracts.SecurityEvents
{
    public abstract record SecurityEventBase
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;

        public Guid? UserId { get; init; }
        public string? UserName { get; init; }
        public string SourceService { get; init; } = default!;
        public string Severity { get; init; } = "High";
    }

}
