using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingCancelledEvent(
        MeetingId MeetingId,
        string Reason,
        string? OutlookEventId,
        IReadOnlyList<Guid> AttendeeIds
    ) : IDomainEvent;
}
