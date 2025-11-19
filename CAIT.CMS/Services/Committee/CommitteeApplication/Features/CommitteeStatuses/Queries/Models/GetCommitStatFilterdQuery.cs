using CommitteeApplication.Common;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using CommitteeApplication.Wrappers;
using MediatR;

namespace CommitteeApplication.Features.CommitteeStatuses.Queries.Models
{
    public class GetCommitStatFilterdQuery
        : PaginationRequest, IRequest<PaginatedResult<GetCommitteeStatusResponse>>
    {
    }
}
