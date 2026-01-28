using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record RemoveAgendaItemAttachmentCommand(Guid MeetingId, Guid AgendaItemId, Guid AttachmentId)
        : ICommand<Result>;
}
