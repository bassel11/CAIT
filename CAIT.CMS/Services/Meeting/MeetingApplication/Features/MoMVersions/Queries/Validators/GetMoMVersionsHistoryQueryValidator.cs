using FluentValidation;
using MeetingApplication.Features.MoMVersions.Queries.Models;

namespace MeetingApplication.Features.MoMVersions.Queries.Validators
{
    public class GetMoMVersionsHistoryQueryValidator : AbstractValidator<GetMoMVersionsHistoryQuery>
    {
        public GetMoMVersionsHistoryQueryValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
        }
    }

    public class GetMoMVersionDetailQueryValidator : AbstractValidator<GetMoMVersionDetailQuery>
    {
        public GetMoMVersionDetailQueryValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.VersionNumber).GreaterThan(0).WithMessage("Version number must be greater than 0.");
        }
    }
}
