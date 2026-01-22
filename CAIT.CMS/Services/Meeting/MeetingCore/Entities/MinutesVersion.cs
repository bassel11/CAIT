using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MinutesVersionVO;
using MeetingCore.ValueObjects.MinutesVO;

namespace MeetingCore.Entities
{
    public class MinutesVersion : Entity<MinutesVersionId>
    {
        public MoMId MoMId { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public int VersionNumber { get; private set; }

        // استخدام UserId بدلاً من Guid
        public UserId ModifiedBy { get; private set; } = default!;
        public DateTime ModifiedAt { get; private set; }

        private MinutesVersion() { }

        // Internal Constructor
        // يتم الإنشاء فقط عند قيام MinutesOfMeeting بعمل UpdateContent
        internal MinutesVersion(
            MoMId momId,
            string content,
            int versionNumber,
            UserId modifiedBy)
        {
            Id = MinutesVersionId.Of(Guid.NewGuid());
            MoMId = momId;
            Content = content;
            VersionNumber = versionNumber;
            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;

            // تعبئة بيانات الـ Entity Base Audit
            CreatedBy = modifiedBy.Value.ToString();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
