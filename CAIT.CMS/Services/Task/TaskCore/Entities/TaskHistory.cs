using TaskCore.Enums;

namespace TaskCore.Entities
{
    public class TaskHistory : Entity<TaskHistoryId>
    {
        public TaskItemId TaskItemId { get; private set; } = default!;
        public UserId UserId { get; private set; } = default!; // من قام بالفعل
        public TaskHistoryAction Action { get; private set; } = TaskHistoryAction.Created;
        public string Details { get; private set; } = default!; // وصف مقروء (مثلاً: Changed status from Open to InProgress)

        // خياري: تخزين القيم القديمة والجديدة إذا أردت دقة عالية
        public string? OldValue { get; private set; }
        public string? NewValue { get; private set; }

        public DateTime Timestamp { get; private set; }

        private TaskHistory() { } // For EF Core

        internal TaskHistory(
            TaskItemId taskItemId,
            UserId userId,
            TaskHistoryAction action,
            string details,
            string? oldValue = null,
            string? newValue = null)
        {
            if (taskItemId is null) throw new DomainException("TaskItemId is required.");
            if (userId is null) throw new DomainException("UserId is required.");
            if (string.IsNullOrWhiteSpace(details)) throw new DomainException("History details cannot be empty.");

            Id = TaskHistoryId.Of(Guid.NewGuid());
            TaskItemId = taskItemId;
            UserId = userId;
            Action = action;
            Details = details;
            OldValue = oldValue;
            NewValue = newValue;
            Timestamp = DateTime.UtcNow;
        }
    }
}
