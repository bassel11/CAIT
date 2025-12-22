namespace TaskCore.Entities
{
    public class TaskNote : Entity<TaskNoteId>
    {
        public TaskItemId TaskItemId { get; private set; } = default!;
        public TaskItem TaskItem { get; private set; } = default!;
        public UserId UserId { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = false;

        private TaskNote() { } // For EF Core

        // التعديل: جعلناه internal ويستقبل Value Objects
        internal TaskNote(TaskItemId taskItemId, UserId userId, string content)
        {
            if (taskItemId is null) throw new DomainException("TaskItemId is required.");
            if (userId is null) throw new DomainException("UserId is required.");
            if (string.IsNullOrWhiteSpace(content)) throw new DomainException("Note content cannot be empty.");

            Id = TaskNoteId.Of(Guid.NewGuid());
            TaskItemId = taskItemId; // ✅ ربط الملاحظة بالمهمة
            UserId = userId;         // ✅ استخدام Value Object
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        internal void UpdateContent(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent)) throw new DomainException("Content cannot be empty.");

            Content = newContent;
            LastTimeModified = DateTime.UtcNow;
        }

        // دالة الحذف الناعم (Internal)
        internal void MarkAsDeleted()
        {
            IsDeleted = true;
            LastTimeModified = DateTime.UtcNow;
        }
    }
}
