using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMActionItemDrafts.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMActionItemDraftVO;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Handlers
{
    public class UpdateActionItemDraftCommandHandler : ICommandHandler<UpdateActionItemDraftCommand, Result>
    {
        private readonly IMinutesRepository _repo;

        public UpdateActionItemDraftCommandHandler(IMinutesRepository repo) => _repo = repo;

        public async Task<Result> Handle(UpdateActionItemDraftCommand req, CancellationToken ct)
        {
            // ✅ تحميل المهام فقط
            var mom = await _repo.GetWithActionItemsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.UpdateActionItem(
                    MoMActionItemDraftId.Of(req.ActionItemId),
                    req.TaskTitle,
                    req.AssigneeId,
                    req.DueDate
                );

                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Action item updated successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
