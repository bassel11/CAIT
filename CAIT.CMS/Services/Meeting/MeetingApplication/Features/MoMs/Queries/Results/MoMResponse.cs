using MeetingApplication.Features.MoMAttachments.Queries.Results;

namespace MeetingApplication.Features.MoMs.Queries.Results
{
    public class MoMResponse
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public int VersionNumber { get; set; }
        public string Status { get; set; } = null!;
        public string? Summary { get; set; }
        public string? Decisions { get; set; }
        public string? DiscussionPoints { get; set; }
        public string? ActionItemsJson { get; set; }
        public string? TeamTranscriptRef { get; set; }
        public List<MoMAttachmentResponse> Attachments { get; set; } = new();
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? DistributedAt { get; set; }
    }
}
