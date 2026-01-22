using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMDecisionDrafts.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMDecisionDraftVO;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Handlers
{
    public class RemoveDecisionDraftCommandHandler
        : ICommandHandler<RemoveDecisionDraftCommand, Result>
    {
        private readonly IMinutesRepository _repo;

        public RemoveDecisionDraftCommandHandler(IMinutesRepository repo) => _repo = repo;

        public async Task<Result> Handle(RemoveDecisionDraftCommand req, CancellationToken ct)
        {
            // نحتاج تحميل القرارات لنتمكن من الحذف من القائمة
            var mom = await _repo.GetWithDecisionsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.RemoveDecision(MoMDecisionDraftId.Of(req.DecisionId));
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Decision removed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
