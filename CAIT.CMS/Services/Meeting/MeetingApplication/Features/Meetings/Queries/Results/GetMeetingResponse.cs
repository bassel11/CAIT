namespace MeetingApplication.Features.Meetings.Queries.Results
{
    public class GetMeetingResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; } = default!;
        public string Status { get; set; } = default!;

        public string LocationType { get; set; } = default!;
        public string? LocationRoom { get; set; }
        public string? LocationAddress { get; set; }
        public string? LocationOnlineUrl { get; set; }
        public string FormattedLocation { get; set; } = default!; // حقل جاهز للعرض

        public bool IsRecurring { get; set; }
        public string? RecurrenceType { get; set; }
        public string? TeamsLink { get; set; }
        public string? OutlookEventId { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        // إحصائيات خفيفة
        public int AttendeesCount { get; set; }
        public int AgendaItemsCount { get; set; }
    }
}
