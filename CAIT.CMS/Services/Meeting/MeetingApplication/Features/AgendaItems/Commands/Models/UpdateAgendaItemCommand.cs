using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record UpdateAgendaItemCommand(
    Guid MeetingId, // ضروري للوصول للـ Aggregate
    Guid AgendaItemId,
    string Title,
    string? Description,
    int SortOrder,
    int? DurationMinutes,
    Guid? PresenterId
) : ICommand<Result>;
}
