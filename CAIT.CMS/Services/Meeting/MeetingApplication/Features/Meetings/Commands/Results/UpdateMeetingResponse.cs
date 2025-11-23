namespace MeetingApplication.Features.Meetings.Commands.Results
{
    public class UpdateMeetingResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        public bool IsRecurring { get; set; }
        public string RecurrenceType { get; set; } = null!;
        public string? TeamsLink { get; set; }
        public string? OutlookEventId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
