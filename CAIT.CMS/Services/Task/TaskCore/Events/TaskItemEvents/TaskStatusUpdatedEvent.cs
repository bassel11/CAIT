namespace TaskCore.Events.TaskItemEvents
{
    public record TaskStatusUpdatedEvent(
        TaskItemId TaskId,
        UserId UpdatedByUserId, // من قام بالتغيير (موظف أو النظام)
        Enums.TaskStatus OldStatus,
        Enums.TaskStatus NewStatus
    ) : IDomainEvent;
}
