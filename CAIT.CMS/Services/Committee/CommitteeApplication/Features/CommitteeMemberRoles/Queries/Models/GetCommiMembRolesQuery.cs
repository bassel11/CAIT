using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Queries.Models
{
    public class GetCommiMembRolesQuery : IRequest<IEnumerable<GetCommiMembRolesResponse>>
    {
        public Guid CommitteeMemberId { get; set; }

        public GetCommiMembRolesQuery(Guid committeeMemberId)
        {
            CommitteeMemberId = committeeMemberId;
        }
    }
}
