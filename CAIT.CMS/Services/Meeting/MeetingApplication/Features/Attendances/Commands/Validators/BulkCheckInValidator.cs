using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class BulkCheckInValidator : AbstractValidator<BulkCheckInCommand>
    {
        public BulkCheckInValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.Entries).NotEmpty();
            RuleForEach(x => x.Entries).ChildRules(entries =>
            {
                entries.RuleFor(e => e.MemberId).NotEmpty();
                entries.RuleFor(e => e.Status).IsInEnum();
            });
        }
    }
}
