using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class RescheduleMeetingCommandHandler : ICommandHandler<RescheduleMeetingCommand, Result>
    {
        private readonly IMeetingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public RescheduleMeetingCommandHandler(
            IMeetingRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(RescheduleMeetingCommand request, CancellationToken cancellationToken)
        {
            var meetingId = MeetingId.Of(request.Id);
            var meeting = await _repository.GetByIdAsync(meetingId, cancellationToken);

            if (meeting == null) return Result.Failure("Meeting not found.");


            // استدعاء السلوك الموجود مسبقاً في Meeting.cs
            meeting.Reschedule(
                request.NewStartDate,
                request.NewEndDate,
                _currentUserService.UserId.ToString()
            );

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Meeting rescheduled successfully.");

        }
    }
}
