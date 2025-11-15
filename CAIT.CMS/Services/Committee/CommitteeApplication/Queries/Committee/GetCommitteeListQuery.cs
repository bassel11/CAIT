using CommitteeApplication.Responses;
using MediatR;

namespace CommitteeApplication.Queries.Committee
{
    public class GetCommitteeListQuery : IRequest<List<CommitteeResponse>>
    {
        public Guid Id { get; set; }
        public GetCommitteeListQuery(Guid id)
        {
            Id = id;
        }
    }
}
