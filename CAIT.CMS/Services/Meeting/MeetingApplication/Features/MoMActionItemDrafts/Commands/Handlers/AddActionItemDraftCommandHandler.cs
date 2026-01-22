using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMActionItemDrafts.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Handlers
{
    public class AddActionItemDraftCommandHandler :
    ICommandHandler<AddActionItemDraftCommand, Result>
    {
        private readonly IMinutesRepository _repo;

        public AddActionItemDraftCommandHandler(IMinutesRepository repo) => _repo = repo;
        public async Task<Result> Handle(AddActionItemDraftCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetWithActionItemsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.AddActionItem(req.TaskTitle, req.AssigneeId, req.DueDate);
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Action item draft added.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }
}
