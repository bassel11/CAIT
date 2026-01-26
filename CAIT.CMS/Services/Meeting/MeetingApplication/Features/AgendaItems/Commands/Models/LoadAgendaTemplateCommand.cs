using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record LoadAgendaTemplateCommand(Guid MeetingId, string TemplateName)
        : ICommand<Result>;
}
