namespace BuildingBlocks.Contracts.Task.IntegrationEvents
{
    public record TaskEscalatedIntegrationEvent
    {
        public Guid TaskId { get; init; }
        public string TaskTitle { get; init; } = default!;
        public DateTime OriginalDeadline { get; init; }
        public int DaysOverdue { get; init; }
        public Guid CommitteeId { get; init; }
        // نرسل معرفات المستخدمين فقط، وخدمة الإشعارات تتصرف (أو نرسل الإيميلات إذا كانت متوفرة لدينا)
        public List<Guid> AssigneeIds { get; init; } = new();
    }
}
