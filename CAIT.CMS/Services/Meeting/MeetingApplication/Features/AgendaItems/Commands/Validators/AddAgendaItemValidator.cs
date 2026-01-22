using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class AddAgendaItemValidator : AbstractValidator<AddAgendaItemCommand>
    {
        public AddAgendaItemValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.SortOrder).GreaterThan(0);
            RuleFor(x => x.DurationMinutes).GreaterThan(0).When(x => x.DurationMinutes.HasValue);
        }
    }
}
