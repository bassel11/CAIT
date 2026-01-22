using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Queries.Results;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record GenerateAgendaByAICommand(
        Guid MeetingId,
        string Purpose
    ) : ICommand<Result<List<AgendaItemResponse>>>;
}
