using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Interfaces.Committee;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class RefreshMeetingQuorumCommandHandler : ICommandHandler<RefreshMeetingQuorumCommand, Result<string>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICommitteeService _committeeService;
        private readonly ICurrentUserService _currentUserService;

        public RefreshMeetingQuorumCommandHandler(
            IMeetingRepository meetingRepository,
            ICommitteeService committeeService,
            ICurrentUserService currentUserService)
        {
            _meetingRepository = meetingRepository;
            _committeeService = committeeService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(RefreshMeetingQuorumCommand request, CancellationToken ct)
        {
            // 1. جلب الاجتماع
            var meetingId = MeetingId.Of(request.MeetingId);
            var meeting = await _meetingRepository.GetByIdAsync(meetingId, ct);

            if (meeting == null)
                return Result<string>.Failure("Meeting not found.");

            // 2. الاتصال بخدمة اللجان لجلب "أحدث" قاعدة سارية
            var ruleDto = await _committeeService.GetQuorumRuleAsync(meeting.CommitteeId.Value, ct);

            if (ruleDto == null)
            {
                return Result<string>.Failure("No active quorum rule found for this committee in the system.");
            }

            // 3. تحويل الـ DTO إلى Value Object (Snapshot جديد)
            // نستخدم ThresholdPercent كما صححنا سابقاً
            var newPolicy = MeetingQuorumPolicy.Create(
                ruleDto.Type,
                ruleDto.ThresholdPercent,
                ruleDto.AbsoluteCount,
                ruleDto.UsePlusOne
            );

            try
            {
                // 4. تنفيذ التحديث في الدومين
                meeting.RefreshQuorumPolicy(newPolicy, _currentUserService.UserId.ToString());

                // 5. الحفظ
                await _meetingRepository.UnitOfWork.SaveChangesAsync(ct);

                return Result<string>.Success($"Quorum policy refreshed successfully to: {newPolicy.GetDescription()}");
            }
            catch (Exception ex)
            {
                // التقاط أخطاء الدومين (مثل محاولة تحديث اجتماع منتهي)
                return Result<string>.Failure(ex.Message);
            }
        }
    }
}
