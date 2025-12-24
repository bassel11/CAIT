namespace TaskCore.Events.TaskItemEvents
{
    public record TaskDetailsUpdatedEvent(
        TaskItemId TaskId,
        UserId ModifierId
    ) : IDomainEvent;
}
