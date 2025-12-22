using TaskCore.Entities;

namespace TaskCore.Events.TaskItemEvents
{
    public record TaskItemCreatedEvent(TaskItem TaskItem) : IDomainEvent;
}
