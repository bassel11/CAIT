using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Handlers
{
    public class UpdateAgendaTemplateCommandHandler
        : ICommandHandler<UpdateAgendaTemplateCommand, Result>
    {
        private readonly IAgendaTemplateRepository _repository;

        public UpdateAgendaTemplateCommandHandler(IAgendaTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(UpdateAgendaTemplateCommand request, CancellationToken ct)
        {
            var template = await _repository.GetByIdAsync(AgendaTemplateId.Of(request.Id), ct);
            if (template == null) return Result.Failure("Template not found.");

            // 1. تحديث البيانات الأساسية (نحتاج إضافة دالة Update في الكيان)
            template.UpdateDetails(request.Name, request.Description);

            // 2. تحديث البنود (Clear & Add)
            // هذا يتطلب دالة في الدومين لتنظيف البنود وإضافتها من جديد
            template.ClearItems();

            foreach (var item in request.Items)
            {
                template.AddItem(item.Title, item.DurationMinutes, item.Description, item.SortOrder);
            }

            await _repository.UnitOfWork.SaveChangesAsync(ct);
            return Result.Success("Template updated.");
        }
    }
}
