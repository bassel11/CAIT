using FluentValidation;
using MeetingApplication.Features.MoMActionItemDrafts.Queries.Models;

namespace MeetingApplication.Features.MoMActionItemDrafts.Queries.Validators
{
    public class GetMoMActionItemsQueryValidator : AbstractValidator<GetMoMActionItemsQuery>
    {
        public GetMoMActionItemsQueryValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            //RuleFor(x => x.PageNumber).GreaterThan(0);
            //RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }
}
