using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Queries.Models;
using MeetingApplication.Features.AgendaTemplates.Queries.Results;
using MeetingCore.Repositories; // ✅ حل مشكلة Repository Namespace
using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingApplication.Features.AgendaTemplates.Queries.Handlers
{
    public class GetAgendaTemplateByIdQueryHandler : IQueryHandler<GetAgendaTemplateByIdQuery, Result<AgendaTemplateResponse>>
    {
        // ✅ استخدام المستودع المخصص بدلاً من العام لضمان الـ Includes والمنطق
        private readonly IAgendaTemplateRepository _repository;

        public GetAgendaTemplateByIdQueryHandler(IAgendaTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<AgendaTemplateResponse>> Handle(GetAgendaTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var id = AgendaTemplateId.Of(request.Id);

            // ✅ الاستدعاء أصبح نظيفاً جداً، تفاصيل الـ EF Core مخفية في الـ Repository
            var template = await _repository.GetByIdAsync(id, cancellationToken);

            if (template == null)
                return Result<AgendaTemplateResponse>.Failure("Template not found.");

            // التحويل إلى DTO
            var response = new AgendaTemplateResponse(
                template.Id.Value,
                template.Name,
                template.Description,
                // ✅ حل مشكلة DateTime (إذا كنت قد أضفت التاريخ للـ Response)
                // template.CreatedAt ?? DateTime.UtcNow, 

                template.Items.Select(i => new AgendaTemplateItemResponse(
                    i.Title,
                    i.DurationMinutes,
                    i.Description,
                    i.SortOrder
                )).OrderBy(x => x.SortOrder).ToList()
            );

            return Result<AgendaTemplateResponse>.Success(response);
        }
    }
}