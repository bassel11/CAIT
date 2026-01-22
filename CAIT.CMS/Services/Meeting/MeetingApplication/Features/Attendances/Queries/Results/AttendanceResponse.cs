namespace MeetingApplication.Features.Attendances.Queries.Results
{
    public class AttendanceResponse
    {
        public Guid UserId { get; set; } // تم تغيير الاسم من MemberId ليتوافق مع الدومين
        public Guid MeetingId { get; set; }
        public string Role { get; set; } = default!;
        public string VotingRight { get; set; } = default!;
        public string RSVP { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime? CheckInTime { get; set; }

        // يمكن إضافة اسم العضو هنا إذا تم عمل JOIN مع خدمة المستخدمين
        // public string? MemberName { get; set; }
    }
}
