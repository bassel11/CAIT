namespace MeetingCore.Entities
{
    public class MinutesVersion
    {
        public Guid Id { get; set; }

        public Guid MoMId { get; set; }
        public MinutesOfMeeting MoM { get; set; } = default!;

        public string Content { get; set; } = default!;
        public int VersionNumber { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
