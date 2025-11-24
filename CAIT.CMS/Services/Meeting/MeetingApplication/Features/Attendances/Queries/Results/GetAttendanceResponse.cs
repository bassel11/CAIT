namespace MeetingApplication.Features.Attendances.Queries.Results
{
    public class GetAttendanceResponse
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public Guid MemberId { get; set; }
        public string RSVP { get; set; } = null!;
        public string AttendanceStatus { get; set; } = null!;
        public DateTime? Timestamp { get; set; }
    }
}
