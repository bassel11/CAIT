using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeStatuses.Queries.Models
{
    public class GetCommitteeStatusesQuery : IRequest<IEnumerable<GetCommitteeStatusResponse>>
    {
    }
}
