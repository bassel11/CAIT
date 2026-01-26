using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class LoadAgendaTemplateCommandHandler : ICommandHandler<LoadAgendaTemplateCommand, Result>
    {
        private readonly IMeetingRepository _repository;

        public LoadAgendaTemplateCommandHandler(IMeetingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(LoadAgendaTemplateCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _repository.GetByIdAsync(MeetingId.Of(request.MeetingId), cancellationToken);
            if (meeting == null) return Result.Failure("Meeting not found.");

            // في التطبيق الحقيقي، هذه القوالب تأتي من Database جدول Templates
            // للتبسيط وتنفيذ المتطلب فوراً، سنضعها Hardcoded كما في المثال
            var templateItems = GetTemplateItems(request.TemplateName);

            if (!templateItems.Any())
                return Result.Failure($"Template '{request.TemplateName}' not found.");

            // إضافة البنود
            int order = 1;
            foreach (var item in templateItems)
            {
                // نتأكد أننا لا نكرر الترتيب إذا كان هناك بنود سابقة
                // أو يمكن مسح البنود السابقة أولاً حسب سياسة العمل

                meeting.AddAgendaItem(
                    AgendaItemTitle.Of(item.Title),
                    Duration.FromMinutes(item.Duration),
                    SortOrder.Of(order++),
                    null, // Presenter
                    item.Desc
                );
            }

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success($"Agenda template '{request.TemplateName}' loaded.");
        }

        // محاكاة لخدمة القوالب
        private List<(string Title, string Desc, int Duration)> GetTemplateItems(string templateName)
        {
            return templateName.ToLower() switch
            {
                "standard" => new()
                {
                    ("Opening", "Welcome and Quorum Check", 5),
                    ("Review Previous Decisions", "Reviewing MoM of last meeting", 15),
                    ("New Topics", "Open floor for new topics", 30),
                    ("Closing", "Next meeting schedule", 5)
                },
                "review" => new()
                {
                     ("Project Status Updates", "", 20),
                     ("Budget Utilization", "", 20),
                     ("Risk Review", "", 15)
                },
                _ => new()
            };
        }
    }
}

