using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class PublishMoMCommandHandler :
    ICommandHandler<PublishMoMCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public PublishMoMCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }
        public async Task<Result> Handle(PublishMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByMeetingIdSimpleAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");
            try
            {
                mom.Publish();
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Published successfully.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }
}
