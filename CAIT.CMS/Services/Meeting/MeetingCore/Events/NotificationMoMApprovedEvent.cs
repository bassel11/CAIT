namespace MeetingCore.Events
{
    public class NotificationMoMApprovedEvent
    {
        public string To { get; }
        public string Subject { get; }
        public string Body { get; }
        public NotificationMoMApprovedEvent(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
