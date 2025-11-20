using CommitteeApplication.Common;
using CommitteeApplication.Features.StatusHistories.Queries.Results;
using CommitteeApplication.Wrappers;
using MediatR;

namespace CommitteeApplication.Features.StatusHistories.Queries.Models
{
    public class GetCommitStatusHistoryQuery
        : PaginationRequest, IRequest<PaginatedResult<CommitStatusHistoryResponse>>
    {
        public Guid? CommitteeId { get; set; } = null;

    }
}
