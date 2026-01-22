using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMDecisionDrafts.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMDecisionDraftVO;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Handlers
{
    public class UpdateDecisionDraftCommandHandler
        : ICommandHandler<UpdateDecisionDraftCommand, Result>
    {
        private readonly IMinutesRepository _repo;

        public UpdateDecisionDraftCommandHandler(IMinutesRepository repo) => _repo = repo;

        public async Task<Result> Handle(UpdateDecisionDraftCommand req, CancellationToken ct)
        {
            // ✅ نستخدم الدالة المخصصة لجلب القرارات فقط
            var mom = await _repo.GetWithDecisionsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                // استدعاء الدالة في الدومين
                mom.UpdateDecision(
                    MoMDecisionDraftId.Of(req.DecisionId),
                    req.Title,
                    req.Text
                );

                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Decision updated successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
