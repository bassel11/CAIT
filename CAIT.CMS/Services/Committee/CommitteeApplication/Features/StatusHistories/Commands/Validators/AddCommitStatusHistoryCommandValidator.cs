using CommitteeApplication.Features.StatusHistories.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.StatusHistories.Commands.Validators
{
    public class AddCommitStatusHistoryCommandValidator : AbstractValidator<AddCommitStatusHistoryCommand>
    {
        public AddCommitStatusHistoryCommandValidator()
        {
            // CommitteeId يجب أن يكون معرف صالح
            RuleFor(x => x.CommitteeId)
                .NotEmpty().WithMessage("CommitteeId must be provided.");

            // OldStatusId يجب أن يكون أكبر من صفر
            RuleFor(x => x.OldStatusId)
                .GreaterThan(0).WithMessage("OldStatusId must be greater than 0.");

            // NewStatusId يجب أن يكون أكبر من صفر
            RuleFor(x => x.NewStatusId)
                .GreaterThan(0).WithMessage("NewStatusId must be greater than 0.");

            // NewStatusId لا يمكن أن يكون نفس OldStatusId
            RuleFor(x => x.NewStatusId)
                .NotEqual(x => x.OldStatusId)
                .WithMessage("NewStatusId must be different from OldStatusId.");

            // DecisionText يجب أن يحتوي نص
            RuleFor(x => x.DecisionText)
                .NotEmpty().WithMessage("DecisionText cannot be empty.")
                .MaximumLength(1000).WithMessage("DecisionText cannot exceed 1000 characters.");
        }
    }
}
