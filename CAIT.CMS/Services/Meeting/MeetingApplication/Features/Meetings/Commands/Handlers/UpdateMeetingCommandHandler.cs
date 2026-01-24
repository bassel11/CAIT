using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class UpdateMeetingCommandHandler : ICommandHandler<UpdateMeetingCommand, Result<Guid>>
    {
        private readonly IMeetingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateMeetingCommandHandler(
            IMeetingRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الاجتماع (استخدام الجلب الخفيف لأننا لا نعدل الأجندة أو الحضور)
            var meetingId = MeetingId.Of(request.Id);
            var meeting = await _repository.GetByIdAsync(meetingId, cancellationToken);

            if (meeting == null)
                return Result<Guid>.Failure("Meeting not found.");

            try
            {
                // 2. تحويل Primitives إلى Value Objects
                var title = MeetingTitle.Of(request.Title);

                var location = MeetingLocation.Create(
                    request.LocationType,
                    request.LocationRoom,
                    request.LocationAddress,
                    request.LocationOnlineUrl
                );

                // 3. استدعاء السلوك (Behavior) في الدومين
                meeting.UpdateDetails(
                    title,
                    request.Description,
                    location,
                    _currentUserService.UserId.ToString()
                );

                // 4. الحفظ (Transactional via UnitOfWork)
                await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(meeting.Id.Value, "Meeting updated successfully.");
            }
            catch (DomainException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
