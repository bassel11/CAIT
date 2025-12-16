using BuildingBlocks.Shared.CQRS;
using DecisionApplication.Dtos;
using FluentValidation;

namespace DecisionApplication.Decisions.Commands.UpdateDecision
{
    public record UpdateDecisionCommand(
        Guid DecisionId,
        UpdateDecisionDto Decision
    ) : ICommand<UpdateDecisionResult>;

    public record UpdateDecisionResult(Guid Id);

    public class UpdateDecisionCommandValidator
       : AbstractValidator<UpdateDecisionCommand>
    {
        public UpdateDecisionCommandValidator()
        {
            RuleFor(x => x.DecisionId)
                .NotEmpty()
                .WithMessage("DecisionId is required");

            RuleFor(x => x.Decision.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(250);

            RuleFor(x => x.Decision.ArabicText)
                .NotEmpty()
                .WithMessage("Arabic text is required");

            RuleFor(x => x.Decision.EnglishText)
                .NotEmpty()
                .WithMessage("English text is required");

            RuleFor(x => x.Decision.Type)
                .IsInEnum()
                .WithMessage("Invalid DecisionType");
        }
    }
}
