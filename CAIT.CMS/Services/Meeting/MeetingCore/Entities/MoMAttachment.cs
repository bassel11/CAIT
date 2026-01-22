using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMAttachmentVO;

namespace MeetingCore.Entities
{
    public class MoMAttachment : Entity<MoMAttachmentId>
    {
        public MoMId MoMId { get; private set; } = default!;
        // بيانات الملف
        public string FileName { get; private set; } = default!;
        public string ContentType { get; private set; } = default!;
        public long SizeInBytes { get; private set; }
        public string StoragePath { get; private set; } = default!;

        // بيانات المستخدم كـ Value Object
        public UserId UploadedBy { get; private set; } = default!;
        public DateTime UploadedAt { get; private set; }

        // EF Core Constructor
        private MoMAttachment() { }
        internal MoMAttachment(
            MoMId momId,
            string fileName,
            string contentType,
            long size,
            string storagePath,
            UserId uploadedBy)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("Filename is required.");
            if (string.IsNullOrWhiteSpace(storagePath))
                throw new DomainException("Storage path is required.");

            Id = MoMAttachmentId.Of(Guid.NewGuid()); // توليد ID قوي
            MoMId = momId;
            FileName = fileName;
            ContentType = contentType;
            SizeInBytes = size;
            StoragePath = storagePath;
            UploadedBy = uploadedBy;
            UploadedAt = DateTime.UtcNow;

            // ورثنا CreatedAt و CreatedBy من الأب (Entity)، يمكن تعيينهم هنا أيضاً إذا أردت توحيد الـ Audit
            CreatedBy = uploadedBy.Value.ToString();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
