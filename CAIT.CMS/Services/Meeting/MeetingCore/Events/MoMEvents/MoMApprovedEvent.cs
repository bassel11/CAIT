using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MinutesVO;

namespace MeetingCore.Events.MoMEvents
{
    public record MoMApprovedEvent(
        MoMId MoMId,
        MeetingId MeetingId,
        Guid ApprovedByUserId,
        DateTime ApprovedAt,
        List<DecisionIntegrationDto> Decisions,
        List<TaskIntegrationDto> Tasks
    ) : IDomainEvent;

    public record DecisionIntegrationDto(string Title, string Content);
    public record TaskIntegrationDto(string Title, Guid AssigneeId, DateTime DueDate);
}
