using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class RejectMoMCommandHandler :
    ICommandHandler<RejectMoMCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public RejectMoMCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(RejectMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByMeetingIdSimpleAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");
            try
            {
                mom.Reject(req.Reason, UserId.Of(_user.UserId));
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Rejected successfully.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }

}
