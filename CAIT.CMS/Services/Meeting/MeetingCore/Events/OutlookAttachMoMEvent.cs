namespace MeetingCore.Events
{
    public class OutlookAttachMoMEvent
    {
        public Guid MeetingId { get; }
        public string Url { get; }

        public OutlookAttachMoMEvent(Guid meetingId, string url)
        {
            MeetingId = meetingId;
            Url = url;
        }
    }
}
