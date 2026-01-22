using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record AddAgendaItemCommand(
    Guid MeetingId,
    string Title,
    string? Description,
    int SortOrder,
    int? DurationMinutes,
    Guid? PresenterId
) : ICommand<Result<Guid>>;
}
