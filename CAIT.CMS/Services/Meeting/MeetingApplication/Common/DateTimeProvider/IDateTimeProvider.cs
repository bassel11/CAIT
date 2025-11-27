namespace MeetingApplication.Common.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }

}
