using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Interfaces.Committee
{
    public class CommitteeMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }

        // هذه هي القيم التي يحتاجها Meeting Aggregate
        public AttendanceRole Role { get; set; }
        public VotingRight VotingRight { get; set; }
    }
}
