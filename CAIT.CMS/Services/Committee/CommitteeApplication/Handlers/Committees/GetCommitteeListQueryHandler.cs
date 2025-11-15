using AutoMapper;
using CommitteeApplication.Queries.Committee;
using CommitteeApplication.Responses;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Handlers.Committees
{
    public class GetCommitteeListQueryHandler : IRequestHandler<GetCommitteeListQuery, List<CommitteeResponse>>
    {
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IMapper _mapper;

        public GetCommitteeListQueryHandler(ICommitteeRepository committeeRepository, IMapper mapper)
        {
            _committeeRepository = committeeRepository;
            _mapper = mapper;
        }
        public async Task<List<CommitteeResponse>> Handle(GetCommitteeListQuery request, CancellationToken cancellationToken)
        {
            var committeeList = await _committeeRepository.GetCommitteesById(request.Id);
            return _mapper.Map<List<CommitteeResponse>>(committeeList);
        }
    }
}
