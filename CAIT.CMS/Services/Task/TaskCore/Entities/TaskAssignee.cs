namespace TaskCore.Entities
{
    public class TaskAssignee : Entity<TaskAssigneeId>
    {
        public TaskItemId TaskItemId { get; private set; } = default!;
        public TaskItem TaskItem { get; private set; } = default!;
        public UserId UserId { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public DateTime AssignedAt { get; private set; }
        private TaskAssignee() { } // For EF Core
        internal TaskAssignee(TaskItemId taskItemId, UserId userId, string name, string email)
        {
            // إضافة: Guard Clauses (حماية الدومين من البيانات الفاسدة)
            if (taskItemId is null) throw new DomainException("TaskItemId is required.");
            if (userId is null) throw new DomainException("UserId is required.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Assignee name cannot be empty.");
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Assignee email cannot be empty.");

            Id = TaskAssigneeId.Of(Guid.NewGuid());
            TaskItemId = taskItemId;
            UserId = userId;
            Name = name;
            Email = email;
            AssignedAt = DateTime.UtcNow;
        }
    }
}
