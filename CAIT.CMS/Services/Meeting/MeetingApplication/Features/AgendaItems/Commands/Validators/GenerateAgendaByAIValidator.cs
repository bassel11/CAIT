using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class GenerateAgendaByAIValidator : AbstractValidator<GenerateAgendaByAICommand>
    {
        public GenerateAgendaByAIValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.Purpose)
                .NotEmpty().WithMessage("Purpose is required for AI generation.")
                .MaximumLength(1000).WithMessage("Purpose creates a prompt context and should be concise.");
        }
    }
}
