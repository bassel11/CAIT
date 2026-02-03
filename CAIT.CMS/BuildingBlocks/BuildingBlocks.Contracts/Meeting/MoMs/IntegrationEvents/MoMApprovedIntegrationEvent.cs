namespace BuildingBlocks.Contracts.Meeting.MoMs.IntegrationEvents
{
    // هذا هو الحدث الذي سيخرج للعالم الخارجي
    // لاحظ أننا نستخدم أنواع بيانات بسيطة ولا نستخدم ValueObjects الخاصة بالدومين
    public record MoMApprovedIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public Guid MeetingId { get; init; }
        public Guid MoMId { get; init; }
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        // القوائم التي تهم الخدمات الأخرى
        public List<DecisionItemDto> Decisions { get; init; } = new();
        public List<TaskItemDto> Tasks { get; init; } = new();
    }

    public record DecisionItemDto(string Title, string Content);

    public record TaskItemDto(string Title, Guid AssigneeId, DateTime DueDate);
}
