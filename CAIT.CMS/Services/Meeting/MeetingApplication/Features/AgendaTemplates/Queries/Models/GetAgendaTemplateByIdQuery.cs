using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Queries.Results;

namespace MeetingApplication.Features.AgendaTemplates.Queries.Models
{
    public record GetAgendaTemplateByIdQuery(Guid Id)
        : IQuery<Result<AgendaTemplateResponse>>;
}
