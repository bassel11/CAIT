using BuildingBlocks.Shared.Exceptions;
using DecisionCore.Enums;

namespace DecisionApplication.Decisions.Commands.UpdateDecision
{
    public class UpdateDecisionHandler
        : ICommandHandler<UpdateDecisionCommand, UpdateDecisionResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateDecisionHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UpdateDecisionResult> Handle(
            UpdateDecisionCommand command,
            CancellationToken cancellationToken)
        {
            var decisionId = DecisionId.Of(command.DecisionId);

            var decision = await _dbContext.Decisions
                .FirstOrDefaultAsync(d => d.Id == decisionId, cancellationToken);

            if (decision is null)
                throw new NotFoundException("Decision", command.DecisionId);

            UpdateDecisionWithNewValues(decision, command.Decision);

            _dbContext.Decisions.Update(decision);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateDecisionResult(decision.Id.Value);
        }

        private static void UpdateDecisionWithNewValues(
            Decision decision,
            UpdateDecisionDto dto)
        {
            // ❌ لا نسمح بالتعديل بعد الإقرار النهائي
            if (decision.Status is DecisionStatus.Approved or DecisionStatus.Rejected)
                throw new DomainException("Cannot update a decision that has been finalized (Approved or Rejected).");

            decision.Update(
                title: DecisionTitle.Of(dto.Title),
                text: DecisionText.Of(dto.ArabicText, dto.EnglishText),
                type: dto.Type,
                agendaItemId: dto.AgendaItemId != null
                    ? AgendaItemId.Of(dto.AgendaItemId.Value)
                    : null
            );
        }
    }
}
