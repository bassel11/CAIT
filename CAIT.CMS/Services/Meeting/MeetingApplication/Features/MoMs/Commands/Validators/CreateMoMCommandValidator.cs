using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class CreateMoMCommandValidator : AbstractValidator<CreateMoMCommand>
    {
        public CreateMoMCommandValidator()
        {
            RuleFor(x => x.MeetingId)
                .NotEmpty().WithMessage("MeetingId is required.");

            RuleFor(x => x.InitialContent)
                .NotEmpty().WithMessage("Initial content cannot be empty.")
                .MaximumLength(1000000).WithMessage("Content is too long."); // تحديد سقف للحماية
        }
    }
}
