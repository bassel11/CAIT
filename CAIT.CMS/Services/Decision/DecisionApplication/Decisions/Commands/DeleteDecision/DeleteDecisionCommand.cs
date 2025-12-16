using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace DecisionApplication.Decisions.Commands.DeleteDecision
{
    public record DeleteDecisionCommand(Guid DecisionId)
        : ICommand<DeleteDecisionResult>;

    public record DeleteDecisionResult(bool IsDeleted);

    namespace DecisionApplication.Decisions.Commands.DeleteDecision
    {
        public class DeleteDecisionCommandValidator
            : AbstractValidator<DeleteDecisionCommand>
        {
            public DeleteDecisionCommandValidator()
            {
                RuleFor(x => x.DecisionId)
                    .NotEmpty()
                    .WithMessage("DecisionId is required");
            }
        }
    }
}
