using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CompleteMeetingCommandHandler : ICommandHandler<CompleteMeetingCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICurrentUserService _currentUserService;

        public CompleteMeetingCommandHandler(
            IMeetingRepository meetingRepository,
            ICurrentUserService currentUserService)
        {
            _meetingRepository = meetingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(CompleteMeetingCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الاجتماع (استخدام GetByIdAsync الخفيفة يكفي هنا)
            var meetingId = MeetingId.Of(request.Id);
            var meeting = await _meetingRepository.GetByIdAsync(meetingId, cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            // 2. تنفيذ السلوك (Domain Logic)
            // هذا السطر يقوم بكل العمل الشاق: التحقق، تغيير الحالة، وإضافة الحدث
            meeting.Complete(_currentUserService.UserId.ToString());

            // 3. الحفظ (Commit)
            // سيتم حفظ الحالة الجديدة + إرسال الحدث MeetingCompletedEvent للـ Outbox
            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Meeting completed successfully.");

        }
    }
}
