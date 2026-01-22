using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class DeleteAgendaItemValidator : AbstractValidator<DeleteAgendaItemCommand>
    {
        public DeleteAgendaItemValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty().WithMessage("MeetingId is required.");
            RuleFor(x => x.AgendaItemId).NotEmpty().WithMessage("AgendaItemId is required.");
        }
    }
}
