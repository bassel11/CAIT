using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories; // ✅ استخدام المستودع المخصص
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.AgendaTemplateVO; // تأكد من الـ Namespaces الجديدة
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class LoadAgendaTemplateCommandHandler : ICommandHandler<LoadAgendaTemplateCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;
        // ✅ التغيير الجوهري: استخدام المستودع المخصص لضمان تحميل البيانات المرتبطة (Items)
        private readonly IAgendaTemplateRepository _templateRepository;

        public LoadAgendaTemplateCommandHandler(
            IMeetingRepository meetingRepository,
            IAgendaTemplateRepository templateRepository)
        {
            _meetingRepository = meetingRepository;
            _templateRepository = templateRepository;
        }

        public async Task<Result> Handle(LoadAgendaTemplateCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الاجتماع مع الأجندة الحالية (استعلام محسن في MeetingRepository)
            var meetingId = MeetingId.Of(request.MeetingId);
            var meeting = await _meetingRepository.GetWithAgendaAsync(meetingId, cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            // 2. جلب القالب باستخدام المستودع المخصص
            // ✅ هذا السطر ينفذ الاستعلام في قاعدة البيانات ويجلب الـ Items عبر Include المخفي داخل المستودع
            var templateId = AgendaTemplateId.Of(request.TemplateId);
            var template = await _templateRepository.GetByIdAsync(templateId, cancellationToken);

            if (template == null)
                return Result.Failure("Template not found.");

            if (!template.Items.Any())
                return Result.Failure("Template is empty.");

            // 3. حساب ترتيب البدء (Logic in Memory - سريع جداً)
            int orderCounter = meeting.AgendaItems.Any()
                ? meeting.AgendaItems.Max(i => i.SortOrder.Value) + 1
                : 1;

            // 4. نسخ البنود (Mapping)
            // بما أن template.Items محملة بالفعل (Eager Loaded)، الحلقة هنا لا تسبب استعلامات إضافية
            foreach (var item in template.Items.OrderBy(x => x.SortOrder))
            {
                meeting.AddAgendaItem(
                    AgendaItemTitle.Of(item.Title),
                    Duration.FromMinutes(item.DurationMinutes),
                    SortOrder.Of(orderCounter++),
                    null, // لا يوجد مقدم في القالب
                    item.Description
                );
            }

            // 5. حفظ التغييرات (Transaction واحدة)
            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success($"Template '{template.Name}' loaded successfully.");
        }
    }
}