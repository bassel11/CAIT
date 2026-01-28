using FluentValidation;
using MeetingApplication.Features.AgendaTemplates.Commands.Models;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Validators
{
    public class CreateAgendaTemplateValidator : AbstractValidator<CreateAgendaTemplateCommand>
    {
        public CreateAgendaTemplateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.Items).NotEmpty().WithMessage("Template must have at least one item.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.Title).NotEmpty();
                items.RuleFor(i => i.DurationMinutes).GreaterThan(0);
                items.RuleFor(i => i.SortOrder).GreaterThan(0);
            });
        }
    }
}
