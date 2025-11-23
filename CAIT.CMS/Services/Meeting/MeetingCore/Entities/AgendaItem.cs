namespace MeetingCore.Entities
{
    public class AgendaItem
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = default!;

        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
