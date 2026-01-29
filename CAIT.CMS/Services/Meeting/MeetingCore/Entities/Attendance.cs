using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Entities
{
    public class Attendance : Entity<AttendanceId>
    {
        public MeetingId MeetingId { get; set; } = default!;
        public UserId UserId { get; private set; } = default!;

        public AttendanceRole Role { get; private set; } = AttendanceRole.Optional;
        public VotingRight VotingRight { get; private set; } = VotingRight.NonVoting;

        public RSVPStatus RSVP { get; private set; } = RSVPStatus.Pending;
        public AttendanceStatus AttendanceStatus { get; private set; } = AttendanceStatus.None;

        public DateTime? CheckInTime { get; private set; }

        //  إضافة دعم للنائب والملاحظات
        public bool IsProxy { get; private set; } = false;
        public string? ProxyName { get; private set; } // اسم النائب إذا كان من خارج النظام
        public string? Notes { get; private set; }

        private Attendance() { }

        internal Attendance(
            MeetingId meetingId,
            UserId userId,
            AttendanceRole role,
            VotingRight votingRight)
        {
            Id = AttendanceId.Of(Guid.NewGuid());
            MeetingId = meetingId ?? throw new DomainException("MeetingId is required.");
            UserId = userId ?? throw new DomainException("UserId is required.");
            Role = role;
            VotingRight = votingRight;
        }

        public void ConfirmRSVP(RSVPStatus status)
        {
            //if (status == RSVPStatus.Pending)
            //    throw new DomainException("Invalid RSVP transition.");

            // لانمنع التغيير لان الاعضاء احيانا يغيرون رأيهم
            RSVP = status;
        }

        // تعديل جوهري: السماح بالحضور المباشر والنائب
        public void CheckIn(bool isRemote, bool isProxy = false, string? proxyName = null)
        {
            // 1. التعامل مع RSVP: إذا حضر، فهو موافق ضمناً
            if (RSVP != RSVPStatus.Accepted)
            {
                RSVP = RSVPStatus.Accepted;
            }

            // 2. تحديث الحالة
            AttendanceStatus = isRemote
                ? AttendanceStatus.Remote : AttendanceStatus.Present;

            CheckInTime = DateTime.UtcNow;

            // 3. تسجيل النائب
            IsProxy = isProxy;
            if (isProxy)
            {
                if (string.IsNullOrWhiteSpace(proxyName))
                    throw new DomainException("Proxy name is required when checking in via proxy.");

                ProxyName = proxyName;
                Notes = $"Represented by proxy: {proxyName}";
            }
        }

        public void MarkAbsent(string? reason = null)
        {
            AttendanceStatus = AttendanceStatus.Absent;
            Notes = reason;
        }
        public bool CountsForQuorum()
        {
            // يحسب للنصاب إذا كان عضواً مصوتاً وحاضراً (بنفسه أو بنائب)
            return VotingRight == VotingRight.Voting &&
                 (AttendanceStatus == AttendanceStatus.Present
                 || AttendanceStatus == AttendanceStatus.Remote);
        }

        //public bool CountsForQuorum()
        //    => Role == AttendanceRole.Required && VotingRight == VotingRight.Voting;

        internal void SetStatus(AttendanceStatus status, DateTime? checkInTime = null)
        {
            // منطق بسيط لتحديث الحالة
            AttendanceStatus = status;

            // تحديث الوقت:
            // إذا تم تمرير وقت، نستخدمه
            // إذا لم يمرر وكانت الحالة حضوراً، نستخدم الوقت الحالي
            // إذا كانت الحالة غياباً، نصفر الوقت
            if (status != AttendanceStatus.None && status != AttendanceStatus.Absent)
            {
                CheckInTime = checkInTime ?? DateTime.UtcNow;
            }
            else
            {
                CheckInTime = null;
            }
        }

    }
}
