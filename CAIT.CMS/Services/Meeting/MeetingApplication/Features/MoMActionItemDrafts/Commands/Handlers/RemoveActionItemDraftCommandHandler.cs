using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMActionItemDrafts.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMActionItemDraftVO;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Handlers
{
    public class RemoveActionItemDraftCommandHandler : ICommandHandler<RemoveActionItemDraftCommand, Result>
    {
        private readonly IMinutesRepository _repo;

        public RemoveActionItemDraftCommandHandler(IMinutesRepository repo) => _repo = repo;

        public async Task<Result> Handle(RemoveActionItemDraftCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetWithActionItemsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.RemoveActionItem(MoMActionItemDraftId.Of(req.ActionItemId));
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Action item removed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
