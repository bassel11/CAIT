namespace TaskCore.Events.AutomationEvents
{
    // حدث التصعيد (عندما تتأخر المهمة)
    public record TaskEscalatedEvent(
        Guid TaskId,
        Guid CommitteeId,
        string TaskTitle,
        DateTime OriginalDeadline,
        int DaysOverdue,
        List<Guid> AssigneeIds // المسؤولين المتأخرين
    ) : IDomainEvent;
}
