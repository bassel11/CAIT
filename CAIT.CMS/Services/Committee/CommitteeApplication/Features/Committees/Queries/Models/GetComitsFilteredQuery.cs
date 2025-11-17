using CommitteeApplication.Common;
using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Wrappers;
using MediatR;

namespace CommitteeApplication.Features.Committees.Queries.Models
{
    public class GetComitsFilteredQuery
                    : PaginationRequest, IRequest<PaginatedResult<GetComitsFilteredResponse>>
    {
    }
}
