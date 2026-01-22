using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class ScheduleMeetingCommandHandler : ICommandHandler<ScheduleMeetingCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public ScheduleMeetingCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(ScheduleMeetingCommand request, CancellationToken cancellationToken)
        {
            // 1. استخدام دالة الجلب المخصصة لضمان وجود الأجندة والحضور في الذاكرة
            var meetingId = MeetingId.Of(request.Id);
            var meeting = await _meetingRepository.GetForSchedulingAsync(meetingId, cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            // 2. تنفيذ السلوك في الدومين
            // هذا السطر سيفحص القواعد (هل يوجد أجندة؟ هل الحالة Draft؟)
            // وسيقوم بتغيير الحالة وإطلاق الحدث
            meeting.Schedule();

            // 3. الحفظ (Commit)
            // سيتم حفظ الحالة الجديدة + تخزين الحدث في Outbox
            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Meeting scheduled successfully.");

        }
    }
}
