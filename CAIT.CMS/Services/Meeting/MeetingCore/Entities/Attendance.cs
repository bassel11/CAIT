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
            if (status == RSVPStatus.Pending)
                throw new DomainException("Invalid RSVP transition.");

            RSVP = status;
        }

        public void CheckIn(bool isRemote)
        {
            if (RSVP != RSVPStatus.Accepted)
                throw new DomainException("Cannot check-in without accepted RSVP.");

            AttendanceStatus = isRemote
                ? AttendanceStatus.Remote
                : AttendanceStatus.Present;

            CheckInTime = DateTime.UtcNow;
        }

        public bool CountsForQuorum()
            => Role == AttendanceRole.Required && VotingRight == VotingRight.Voting;

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
