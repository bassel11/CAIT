namespace BuildingBlocks.Contracts.SecurityEvents
{
    public record UnauthorizedAccessAttemptEvent : SecurityEventBase
    {
        public string RequestedResource { get; init; } = default!;
        public string HttpMethod { get; init; } = default!;
        public string IpAddress { get; init; } = default!;
    }

}
