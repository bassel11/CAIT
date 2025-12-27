namespace BuildingBlocks.Contracts.Task.IntegrationEvents
{
    public record TaskAssignedIntegrationEvent
    {
        public Guid TaskId { get; init; }
        public Guid CommitteeId { get; init; }

        public Guid MemberId { get; init; }
        public string MemberName { get; init; } = default!;
        public string MemberEmail { get; init; } = default!;

        public DateTime AssignedAtUtc { get; init; }
    }
}
