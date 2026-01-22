using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingScheduledEvent(
        MeetingId MeetingId,
        CommitteeId CommitteeId,
        string Title,
        DateTime StartDate,
        DateTime EndDate,
        IReadOnlyList<Guid> AttendeeIds
    ) : IDomainEvent;
}
