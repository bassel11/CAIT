using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMDecisionDrafts.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Handlers
{
    public class AddDecisionDraftsCommandHandler :
    ICommandHandler<AddDecisionDraftCommand, Result>
    {
        private readonly IMinutesRepository _repo;

        public AddDecisionDraftsCommandHandler(IMinutesRepository repo) => _repo = repo;

        public async Task<Result> Handle(AddDecisionDraftCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetWithDecisionsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.AddDecision(req.Title, req.Text);
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Decision draft added.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }
}
