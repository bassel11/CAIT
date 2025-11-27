namespace MeetingCore.Events
{
    public class MoMPublishedEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid PublishedBy { get; }
        public DateTime PublishedAt { get; }
        public string PublicUrl { get; }

        public MoMPublishedEvent(Guid momId, Guid meetingId, Guid publishedBy, DateTime publishedAt, string publicUrl)
        {
            MoMId = momId;
            MeetingId = meetingId;
            PublishedBy = publishedBy;
            PublishedAt = publishedAt;
            PublicUrl = publicUrl;
        }
    }

}
