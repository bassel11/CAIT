using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class ApproveMoMCommandHandler :
    ICommandHandler<ApproveMoMCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public ApproveMoMCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }
        public async Task<Result> Handle(ApproveMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetFullGraphByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");
            try
            {
                mom.Approve(_user.UserId); // يطلق حدث MoMApprovedEvent
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Approved successfully.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }
}
