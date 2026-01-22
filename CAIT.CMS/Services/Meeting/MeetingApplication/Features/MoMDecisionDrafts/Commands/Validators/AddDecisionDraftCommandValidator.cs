using FluentValidation;
using MeetingApplication.Features.MoMDecisionDrafts.Commands.Models;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Validators
{
    public class AddDecisionDraftCommandValidator : AbstractValidator<AddDecisionDraftCommand>
    {
        public AddDecisionDraftCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Decision title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Decision text is required.");
        }
    }

    public class UpdateDecisionDraftCommandValidator : AbstractValidator<UpdateDecisionDraftCommand>
    {
        public UpdateDecisionDraftCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.DecisionId).NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Decision title is required.")
                .MaximumLength(200);

            RuleFor(x => x.Text).NotEmpty();
        }
    }

    public class RemoveDecisionDraftCommandValidator : AbstractValidator<RemoveDecisionDraftCommand>
    {
        public RemoveDecisionDraftCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.DecisionId).NotEmpty();
        }
    }
}
