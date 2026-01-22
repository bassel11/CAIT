using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class GenerateMoMByAICommandValidator : AbstractValidator<GenerateMoMByAICommand>
    {
        public GenerateMoMByAICommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();

            RuleFor(x => x.Transcript)
                .NotEmpty().WithMessage("Transcript is required for AI generation.")
                .MinimumLength(50).WithMessage("Transcript is too short to generate meaningful content.");
        }
    }
}
