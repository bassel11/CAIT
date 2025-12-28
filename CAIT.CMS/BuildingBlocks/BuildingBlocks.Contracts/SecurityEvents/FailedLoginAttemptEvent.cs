namespace BuildingBlocks.Contracts.SecurityEvents
{
    public record FailedLoginAttemptEvent : SecurityEventBase
    {
        public int FailedCount { get; init; }
        public string IpAddress { get; init; } = default!;
        public bool ThresholdExceeded { get; init; }
    }

}
