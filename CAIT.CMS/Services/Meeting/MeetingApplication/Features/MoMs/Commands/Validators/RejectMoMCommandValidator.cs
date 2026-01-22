using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class RejectMoMCommandValidator : AbstractValidator<RejectMoMCommand>
    {
        public RejectMoMCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Rejection reason is required.")
                .MinimumLength(5).WithMessage("Rejection reason must be at least 5 characters.")
                .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters.");
        }
    }
}
