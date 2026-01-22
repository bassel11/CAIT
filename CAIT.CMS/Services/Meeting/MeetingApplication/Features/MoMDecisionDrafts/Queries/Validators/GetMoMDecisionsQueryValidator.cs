using FluentValidation;
using MeetingApplication.Features.MoMDecisionDrafts.Queries.Models;

namespace MeetingApplication.Features.MoMDecisionDrafts.Queries.Validators
{
    public class GetMoMDecisionsQueryValidator : AbstractValidator<GetMoMDecisionsQuery>
    {
        public GetMoMDecisionsQueryValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            //RuleFor(x => x.PageNumber).GreaterThan(0);
            //RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100); // حماية من استعلامات ضخمة
        }
    }
}
