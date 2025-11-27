namespace MeetingApplication.Features.MoMVersions.Queries.Results
{
    public class MoMVersionResponse
    {
        public Guid Id { get; set; }
        public Guid MoMId { get; set; }
        public int VersionNumber { get; set; }
        public string Content { get; set; } = null!;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
