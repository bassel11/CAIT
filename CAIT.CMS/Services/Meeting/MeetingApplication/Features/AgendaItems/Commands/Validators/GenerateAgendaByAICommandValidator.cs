using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class GenerateAgendaByAICommandValidator : AbstractValidator<GenerateAgendaByAICommand>
    {
        public GenerateAgendaByAICommandValidator()
        {
            RuleFor(x => x.MeetingId)
                .NotEmpty().WithMessage("Meeting ID is required.");

            RuleFor(x => x.MeetingPurpose)
                .NotEmpty().WithMessage("Meeting purpose is required to generate agenda.")
                .MinimumLength(5).WithMessage("Purpose must be at least 5 characters long.");
        }
    }
}
