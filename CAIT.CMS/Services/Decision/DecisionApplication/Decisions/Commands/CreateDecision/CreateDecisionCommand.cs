using BuildingBlocks.Shared.CQRS;
using DecisionApplication.Dtos;
using FluentValidation;

namespace DecisionApplication.Decisions.Commands.CreateDecision
{
    public record CreateDecisionCommand(CreateDecisionDto Decision) : ICommand<CreateDecisionResult>;

    public record CreateDecisionResult(Guid Id);

    public class CreateDecisionCommandValidator : AbstractValidator<CreateDecisionCommand>
    {
        public CreateDecisionCommandValidator()
        {
            RuleFor(x => x.Decision).NotNull().WithMessage("Decision data is required");
            RuleFor(x => x.Decision.Title).NotEmpty().WithMessage("Decision title is required").MaximumLength(200);
            RuleFor(x => x.Decision.TextArabic)
                .NotEmpty()
                .WithMessage("Arabic text is required")
                .MaximumLength(1000);

            RuleFor(x => x.Decision.TextEnglish)
                .NotEmpty()
                .WithMessage("English text is required")
                .MaximumLength(1000);

            RuleFor(x => x.Decision.MeetingId)
                .NotEmpty()
                .WithMessage("MeetingId is required");

            RuleFor(x => x.Decision.Type)
                .IsInEnum()
                .WithMessage("Invalid decision type");
        }
    }
}
