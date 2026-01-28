using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Queries.Models;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.AgendaTemplates.Queries.Handlers
{
    // ... usings ...
    public class GetAllAgendaTemplatesQueryHandler : IQueryHandler<GetAllAgendaTemplatesQuery, Result<List<AgendaTemplateSummaryDto>>>
    {
        private readonly IAgendaTemplateRepository _repository;

        public GetAllAgendaTemplatesQueryHandler(IAgendaTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<AgendaTemplateSummaryDto>>> Handle(GetAllAgendaTemplatesQuery request, CancellationToken ct)
        {
            var templates = await _repository.GetAllAsync(ct);

            var dtos = templates.Select(t => new AgendaTemplateSummaryDto(
                t.Id.Value,
                t.Name,
                t.Description,
                t.CreatedAt
            )).ToList();

            return Result<List<AgendaTemplateSummaryDto>>.Success(dtos);
        }
    }
}
