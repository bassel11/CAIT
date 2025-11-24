using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class GenerateAgendaByAIValidator : AbstractValidator<GenerateAgendaByAICommand>
    {
        public GenerateAgendaByAIValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.Purpose).NotEmpty().MaximumLength(2000);
        }
    }
}
