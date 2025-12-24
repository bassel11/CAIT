namespace BuildingBlocks.Contracts.Task.IntegrationEvents
{
    public record TaskReminderIntegrationEvent
    {
        public Guid TaskId { get; init; }
        public string TaskTitle { get; init; } = default!;
        public DateTime Deadline { get; init; }
        public int DaysRemaining { get; init; } // 7, 3, 1
        public List<Guid> AssigneeIds { get; init; } = new();
    }
}
