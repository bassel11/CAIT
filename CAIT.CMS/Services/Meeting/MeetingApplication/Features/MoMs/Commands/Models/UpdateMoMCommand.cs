using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record UpdateMoMCommand(
        Guid MoMId,
        string Content,
        string? AttendanceSummary = null,
        string? AgendaSummary = null,
        string? DecisionsSummary = null,
        string? ActionItemsJson = null
    ) : IRequest<Guid>;
}
