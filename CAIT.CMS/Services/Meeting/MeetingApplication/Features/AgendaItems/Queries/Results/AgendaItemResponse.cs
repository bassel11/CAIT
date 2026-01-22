namespace MeetingApplication.Features.AgendaItems.Queries.Results
{
    public class AgendaItemResponse
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int? DurationMinutes { get; set; }
        public Guid? PresenterId { get; set; }
    }
}
