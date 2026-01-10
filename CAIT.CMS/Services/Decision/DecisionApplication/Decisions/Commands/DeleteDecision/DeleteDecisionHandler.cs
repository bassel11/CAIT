using BuildingBlocks.Shared.Exceptions;

namespace DecisionApplication.Decisions.Commands.DeleteDecision
{
    public class DeleteDecisionHandler
        : ICommandHandler<DeleteDecisionCommand, DeleteDecisionResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteDecisionHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeleteDecisionResult> Handle(
            DeleteDecisionCommand command,
            CancellationToken cancellationToken)
        {
            var decisionId = DecisionId.Of(command.DecisionId);

            var decision = await _dbContext.Decisions
                .FirstOrDefaultAsync(d => d.Id == decisionId, cancellationToken);

            if (decision is null)
                throw new NotFoundException("Decision", command.DecisionId);

            // منطق الحذف داخل الـ Aggregate
            decision.Delete();

            _dbContext.Decisions.Remove(decision);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteDecisionResult(true);
        }
    }
}
