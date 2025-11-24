using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class DeleteAgendaItemValidator : AbstractValidator<DeleteAgendaItemCommand>
    {
        public DeleteAgendaItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
