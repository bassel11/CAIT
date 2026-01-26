using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingApplication.Interfaces.AI;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class GenerateAgendaByAICommandHandler : ICommandHandler<GenerateAgendaByAICommand, Result<List<string>>>
    {
        private readonly IMeetingRepository _repository;
        private readonly IAiAgendaService _aiService;
        private readonly ICurrentUserService _currentUserService;

        public GenerateAgendaByAICommandHandler(
            IMeetingRepository repository,
            IAiAgendaService aiService,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _aiService = aiService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<string>>> Handle(GenerateAgendaByAICommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الاجتماع
            // نستخدم MeetingId.Of لضمان الـ Type Safety
            var meetingId = MeetingId.Of(request.MeetingId);
            var meeting = await _repository.GetByIdAsync(meetingId, cancellationToken);

            if (meeting == null)
                return Result<List<string>>.Failure("Meeting not found.");

            // 2. استدعاء خدمة الذكاء الاصطناعي
            // نمرر عنوان الاجتماع وهدفه للحصول على نتائج دقيقة
            var aiResult = await _aiService.GenerateAgendaSuggestionsAsync(request.MeetingPurpose, meeting.Title.Value);

            // التحقق من نجاح العملية الخارجية
            if (!aiResult.Succeeded)
                return Result<List<string>>.Failure(aiResult.Message ?? "AI generation failed.");
            // 3. تخزين النتيجة في سجلات التدقيق (Audit) داخل الكيان
            // هذا يحقق المتطلب: "Agenda updates logged in the system (with full audit trail)"

            // تحويل القائمة إلى نص واحد لتخزينه في قاعدة البيانات
            var rawResult = string.Join("\n", aiResult.Data);

            // تحويل معرف المستخدم الحالي إلى Value Object
            var userId = UserId.Of(_currentUserService.UserId);

            // استدعاء دالة الدومين لتسجيل الحدث
            meeting.LogAIGeneration(
                AIContentType.AgendaDraft,
                request.MeetingPurpose, // الـ Prompt الذي كتبه المستخدم
                rawResult,              // النتيجة التي عادت من AI
                "gpt-4o",               // اسم الموديل (يمكن جلبه من الإعدادات لاحقاً)
                userId
            );

            // 4. الحفظ (Commit)
            // ملاحظة مهمة: نحن هنا نحفظ "سجل التدقيق" (AI Log) فقط.
            // لا يتم تعديل جدول أعمال الاجتماع (AgendaItems) تلقائياً.
            // المستخدم سيقوم بمراجعة المقترحات في الواجهة ثم يرسل أمراً آخر لاعتمادها.
            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // إرجاع القائمة للمستخدم ليعرضها في الـ UI
            return Result<List<string>>.Success(aiResult.Data, "Agenda suggestions generated successfully.");
        }
    }
}
