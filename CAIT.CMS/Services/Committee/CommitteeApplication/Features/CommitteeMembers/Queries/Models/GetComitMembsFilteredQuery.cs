using CommitteeApplication.Common;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeApplication.Wrappers;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Models
{
    public class GetComitMembsFilteredQuery
                 : PaginationRequest, IRequest<PaginatedResult<CommitMembsFilterResponse>>
    {
    }
}
