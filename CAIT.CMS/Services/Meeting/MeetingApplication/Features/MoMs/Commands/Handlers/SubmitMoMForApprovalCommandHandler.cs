using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class SubmitMoMForApprovalCommandHandler :
    ICommandHandler<SubmitMoMForApprovalCommand, Result>

    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public SubmitMoMForApprovalCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(SubmitMoMForApprovalCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByMeetingIdSimpleAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");
            try
            {
                mom.SubmitForApproval();
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Submitted for approval.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }

    }
}
