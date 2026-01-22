using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class RescheduleMeetingValidator : AbstractValidator<RescheduleMeetingCommand>
    {
        public RescheduleMeetingValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.NewStartDate)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow).WithMessage("New start date must be in the future.");

            RuleFor(x => x.NewEndDate)
                .NotEmpty()
                .GreaterThan(x => x.NewStartDate).WithMessage("New end date must be after the new start date.");
        }
    }
}
