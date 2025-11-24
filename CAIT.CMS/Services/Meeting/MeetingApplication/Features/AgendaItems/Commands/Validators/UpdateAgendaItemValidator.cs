using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class UpdateAgendaItemValidator : AbstractValidator<UpdateAgendaItemCommand>
    {
        public UpdateAgendaItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
            //RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(1);
        }
    }
}
