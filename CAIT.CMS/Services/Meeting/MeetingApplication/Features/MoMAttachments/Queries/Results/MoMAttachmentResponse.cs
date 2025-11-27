namespace MeetingApplication.Features.MoMAttachments.Queries.Results
{
    public class MoMAttachmentResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string StoragePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
        public Guid UploadedBy { get; set; }
    }
}
