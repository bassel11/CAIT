using BuildingBlocks.Shared.Abstractions;

namespace MeetingCore.Events
{
    public record MoMApprovedEvent(
            Guid MoMId,
            Guid MeetingId,
            Guid UserId,
            DateTime OccurredOn // نمرر التاريخ ليتطابق مع وقت التعديل
        ) : IDomainEvent;
}
