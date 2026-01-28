using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaTemplates.Queries.Models
{
    public record GetAllAgendaTemplatesQuery
        : IQuery<Result<List<AgendaTemplateSummaryDto>>>;

    public record AgendaTemplateSummaryDto(
        Guid Id,
        string Name,
        string Description,
        DateTime? CreatedAt);
}
