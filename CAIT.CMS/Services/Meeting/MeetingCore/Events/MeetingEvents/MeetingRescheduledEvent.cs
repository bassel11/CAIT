using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingRescheduledEvent(
        MeetingId MeetingId,
        DateTime NewStartDate,
        DateTime NewEndDate,
        string? OutlookEventId // ضروري لنعرف أي حدث في الـ Outlook يجب تحديثه
    ) : IDomainEvent;
}
