using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Responses;
using MediatR;

namespace CommitteeApplication.Features.Committees.Queries.Models
{
    public class GetCommitteeListQuery : IRequest<Response<List<GetCommitteeListResponse>>>
    {
    }
}
