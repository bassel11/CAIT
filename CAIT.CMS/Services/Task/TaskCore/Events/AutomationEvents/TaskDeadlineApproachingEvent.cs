namespace TaskCore.Events.AutomationEvents
{
    // حدث التذكير (قبل الموعد)
    public record TaskDeadlineApproachingEvent(
        Guid TaskId,
        string TaskTitle,
        DateTime Deadline,
        int DaysRemaining, // 7, 3, or 1
        List<Guid> AssigneeIds
    ) : IDomainEvent;
}
