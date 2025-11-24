namespace MeetingApplication.Features.AgendaItems.Queries.Results
{
    public class GetAgendaItemResponse
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
