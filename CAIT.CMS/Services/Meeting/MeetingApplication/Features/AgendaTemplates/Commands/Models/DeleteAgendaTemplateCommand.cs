using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Models
{
    public record DeleteAgendaTemplateCommand(Guid Id)
        : ICommand<Result>;
}
