namespace MeetingCore.Entities
{
    public class MoMAttachment
    {
        public Guid Id { get; set; }
        public Guid MoMId { get; set; }
        public MinutesOfMeeting MoM { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string StoragePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public Guid UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
