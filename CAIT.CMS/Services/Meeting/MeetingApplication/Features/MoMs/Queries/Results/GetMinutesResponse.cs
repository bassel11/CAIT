using MeetingApplication.Features.MoMAttachments.Queries.Results;

namespace MeetingApplication.Features.MoMs.Queries.Results
{
    public class GetMinutesResponse
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Status { get; set; } = null!;
        public string? AttendanceSummary { get; set; }
        public string? AgendaSummary { get; set; }
        public string? DecisionsSummary { get; set; }
        public string? ActionItemsJson { get; set; }
        public int VersionNumber { get; set; }
        public List<MoMAttachmentResponse> Attachments { get; set; } = new();
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
