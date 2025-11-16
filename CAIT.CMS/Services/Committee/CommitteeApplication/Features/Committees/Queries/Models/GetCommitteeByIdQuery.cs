using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Responses;
using MediatR;

namespace CommitteeApplication.Features.Committees.Queries.Models
{
    public class GetCommitteeByIdQuery : IRequest<Response<List<GetCommitteeByIdResponse>>>
    {
        public Guid Id { get; set; }
        public GetCommitteeByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
