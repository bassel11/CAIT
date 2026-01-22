using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class UpdateMoMContentCommandValidator : AbstractValidator<UpdateMoMContentCommand>
    {
        public UpdateMoMContentCommandValidator()
        {
            RuleFor(x => x.MeetingId)
                .NotEmpty().WithMessage("MeetingId is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty.")
                .Must(c => c.Length > 10).WithMessage("Content is too short to be valid.");
        }
    }
}
