using TaskCore.Entities;

namespace TaskCore.Events.TaskAssigneeEvents
{
    public record TaskAssignUserEvent(
        TaskItemId TaskId,       // 👈 مفيد جداً للبحث في الـ Audit Log
        TaskAssignee Assignee    // التفاصيل
    ) : IDomainEvent;
}
