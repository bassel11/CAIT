using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMAttendanceVO;

namespace MeetingCore.Entities
{
    public class MoMAttendance : Entity<MoMAttendanceId>
    {
        public MoMId MoMId { get; private set; } = default!;

        // بيانات العضو (نسخ قيم وليس علاقة مباشرة للحفاظ على التاريخ)
        public UserId UserId { get; private set; } = default!;
        public string MemberName { get; private set; } = default!; // الاسم وقت الاجتماع
        public string Role { get; private set; } = default!; // الدور (رئيس، عضو..)

        // حالة الحضور
        public bool IsPresent { get; private set; }
        public AttendanceStatus Status { get; private set; }
        public string? AbsenceReason { get; private set; }
        public string? Notes { get; private set; }

        private MoMAttendance() { }

        internal MoMAttendance(
            MoMId momId,
            UserId userId,
            string memberName,
            string role,
            AttendanceStatus status,
            string? absenceReason,
            string? notes)
        {
            Id = MoMAttendanceId.Of(Guid.NewGuid());
            MoMId = momId;
            UserId = userId;
            MemberName = memberName;
            Role = role;
            Status = status;
            IsPresent = (status == AttendanceStatus.Present || status == AttendanceStatus.Remote);
            AbsenceReason = absenceReason;
            Notes = notes;
        }

        internal void CorrectStatus(AttendanceStatus newStatus, string? notes)
        {
            Status = newStatus;
            IsPresent = (newStatus == AttendanceStatus.Present || newStatus == AttendanceStatus.Remote);

            // تحديث الملاحظات أو سبب الغياب حسب الحالة الجديدة
            if (newStatus == AttendanceStatus.Absent)
            {
                AbsenceReason = notes;
            }
            else
            {
                Notes = notes;
                AbsenceReason = null; // تصفير سبب الغياب إذا أصبح حاضراً
            }
        }
    }
}
