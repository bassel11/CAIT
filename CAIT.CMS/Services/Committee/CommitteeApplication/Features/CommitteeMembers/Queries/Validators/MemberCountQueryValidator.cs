using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Validators
{
    public class MemberCountQueryValidator : AbstractValidator<MemberCountQuery>
    {
        public MemberCountQueryValidator()
        {
            RuleFor(x => x.CommitteeId).NotEmpty();
        }
    }
}
