using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CreateMeetingCommandHandler : ICommandHandler<CreateMeetingCommand, Result<Guid>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateMeetingCommandHandler(
            IMeetingRepository meetingRepository,
            ICurrentUserService currentUserService)
        {
            _meetingRepository = meetingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {

            // =========================================================
            // 1. تحويل Primitives إلى Value Objects (Domain Validation)
            // =========================================================
            var currentUserId = _currentUserService.UserId.ToString();
            var meetingId = MeetingId.Of(Guid.NewGuid());
            var committeeId = CommitteeId.Of(request.CommitteeId);
            var title = MeetingTitle.Of(request.Title);

            // التحقق من المنطقة الزمنية
            TimeZoneId timeZone;
            timeZone = TimeZoneId.Of(request.TimeZone);


            // بناء كائن الموقع
            var location = MeetingLocation.Create(
                request.LocationType,
                request.LocationRoom,
                request.LocationAddress,
                request.LocationOnlineUrl
            );

            // بناء كائن التكرار
            RecurrencePattern recurrence = RecurrencePattern.None;
            if (request.IsRecurring)
            {
                if (!string.IsNullOrWhiteSpace(request.RecurrenceRule))
                {
                    recurrence = RecurrencePattern.WithRule(request.RecurrenceRule);
                }
                else if (request.RecurrenceType.HasValue)
                {
                    recurrence = RecurrencePattern.Simple(request.RecurrenceType.Value);
                }
            }

            // =========================================================
            // 2. إنشاء الـ Aggregate Root باستخدام الـ Factory
            // =========================================================

            var meeting = Meeting.Create(
                meetingId,
                committeeId,
                title,
                request.Description,
                request.StartDate,
                request.EndDate,
                timeZone,
                location,
                recurrence,
                currentUserId // Audit User
            );

            // =========================================================
            // 3. الحفظ وضمان الـ Transactionality
            // =========================================================

            // إضافة للذاكرة
            await _meetingRepository.AddAsync(meeting, cancellationToken);

            // الحفظ النهائي (مع Outbox Messages)
            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(meeting.Id.Value, "Meeting created successfully.");

        }
    }
}
