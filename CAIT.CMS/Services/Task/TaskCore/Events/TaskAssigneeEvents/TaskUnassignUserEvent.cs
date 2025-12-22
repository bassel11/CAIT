using TaskCore.Entities;

namespace TaskCore.Events.TaskAssigneeEvents
{
    public record TaskUnassignUserEvent(
        TaskItemId TaskId,
        TaskAssignee Assignee
    ) : IDomainEvent;
}
