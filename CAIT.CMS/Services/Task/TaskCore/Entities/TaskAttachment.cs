namespace TaskCore.Entities
{
    public class TaskAttachment : Entity<TaskAttachmentId>
    {
        // العلاقات (Relationships)
        public TaskItemId TaskItemId { get; private set; } = default!;
        public virtual TaskItem TaskItem { get; private set; } = default!; // Navigation Property

        // من قام بالرفع
        public UserId UploadedByUserId { get; private set; } = default!;

        // تفاصيل الملف (Flat Properties)
        public string FileName { get; private set; } = default!;
        public string BlobPath { get; private set; } = default!; // ✅ تم تصحيح الاسم من FileBath
        public string ContentType { get; private set; } = default!;
        public long SizeInBytes { get; private set; }

        // الإصدار والتوقيت
        public int Version { get; private set; }
        public DateTime UploadedAt { get; private set; }

        // Constructor فارغ لـ EF Core
        private TaskAttachment() { }

        // Constructor الداخلي (يتم استدعاؤه من TaskItem فقط)
        internal TaskAttachment(
            TaskItemId taskItemId,
            UserId uploadedBy,
            string fileName,
            string blobPath,
            string contentType,
            long sizeInBytes,
            int version)
        {
            // 1. التحقق من صحة البيانات (Guard Clauses)
            if (taskItemId is null) throw new DomainException("TaskItemId is required.");
            if (uploadedBy is null) throw new DomainException("UploadedByUserId is required.");

            if (string.IsNullOrWhiteSpace(fileName)) throw new DomainException("FileName cannot be empty.");
            if (string.IsNullOrWhiteSpace(blobPath)) throw new DomainException("BlobPath is required.");
            if (string.IsNullOrWhiteSpace(contentType)) throw new DomainException("ContentType is required.");

            if (sizeInBytes <= 0) throw new DomainException("File size must be greater than zero.");
            if (version < 1) throw new DomainException("Version must be 1 or higher.");

            // 2. تعيين القيم
            Id = TaskAttachmentId.Of(Guid.NewGuid());
            TaskItemId = taskItemId;
            UploadedByUserId = uploadedBy; // ✅ تم توحيد الاسم

            FileName = fileName;
            BlobPath = blobPath;           // ✅ تم تصحيح الاسم
            ContentType = contentType;
            SizeInBytes = sizeInBytes;

            Version = version;
            UploadedAt = DateTime.UtcNow;
        }
    }
}