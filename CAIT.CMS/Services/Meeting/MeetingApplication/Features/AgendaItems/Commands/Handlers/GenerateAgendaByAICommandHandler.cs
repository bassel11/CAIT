using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using MeetingApplication.Integrations;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class GenerateAgendaByAICommandHandler : ICommandHandler<GenerateAgendaByAICommand, Result<List<AgendaItemResponse>>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IAgendaGeneratorService _aiService;
        private readonly ICurrentUserService _currentUserService;

        public GenerateAgendaByAICommandHandler(
            IMeetingRepository meetingRepository,
            IAgendaGeneratorService aiService,
            ICurrentUserService currentUserService)
        {
            _meetingRepository = meetingRepository;
            _aiService = aiService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<AgendaItemResponse>>> Handle(GenerateAgendaByAICommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الاجتماع
            var meeting = await _meetingRepository.GetByIdAsync(MeetingId.Of(request.MeetingId), cancellationToken);
            if (meeting == null)
                return Result<List<AgendaItemResponse>>.Failure("Meeting not found.");

            // 2. استدعاء خدمة الذكاء الاصطناعي (Abstraction)
            var generatedItems = await _aiService.GenerateAsync(meeting.Title.Value, request.Purpose, cancellationToken);

            // 3. توثيق العملية في الدومين (Audit Log for AI Usage)
            // نقوم بتجميع النتائج كنص JSON أو نص عادي لغرض التخزين في السجل
            var resultSummary = string.Join("; ", generatedItems.Select(x => x.Title));

            meeting.LogAIGeneration(
                AIContentType.AgendaDraft,
                request.Purpose,          // Prompt
                resultSummary,            // Result Summary
                "Mock-GPT-4",             // Model Name
                UserId.Of(_currentUserService.UserId)
            );

            // 4. حفظ سجل التوثيق (وليس البنود نفسها)
            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // 5. تحويل النتائج إلى Response DTO لإعادتها للواجهة (معرفات عشوائية مؤقتة)
            var response = generatedItems.Select((item, index) => new AgendaItemResponse
            {
                Id = Guid.NewGuid(), // معرف مؤقت للعرض فقط
                MeetingId = request.MeetingId,
                Title = item.Title,
                Description = item.Description,
                SortOrder = index + 1,
                DurationMinutes = item.DurationMinutes,
                PresenterId = null
            }).ToList();

            return Result<List<AgendaItemResponse>>.Success(response, "Agenda suggestions generated successfully.");
        }
    }
}
