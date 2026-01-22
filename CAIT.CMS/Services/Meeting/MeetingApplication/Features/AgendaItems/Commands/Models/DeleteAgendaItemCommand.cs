using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record DeleteAgendaItemCommand(Guid MeetingId, Guid AgendaItemId)
        : ICommand<Result>;
}
