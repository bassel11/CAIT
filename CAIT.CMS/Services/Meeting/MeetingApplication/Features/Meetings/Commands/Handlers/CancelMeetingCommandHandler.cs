using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CancelMeetingCommandHandler : ICommandHandler<CancelMeetingCommand, Result>
    {
        private readonly IMeetingRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CancelMeetingCommandHandler(
            IMeetingRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(CancelMeetingCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _repository.GetByIdAsync(MeetingId.Of(request.Id), cancellationToken);
            if (meeting == null) return Result.Failure("Meeting not found.");

            // استدعاء السلوك
            meeting.Cancel(request.Reason, _currentUserService.UserId.ToString());

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Meeting cancelled successfully.");

        }
    }
}
