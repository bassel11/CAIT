using AutoMapper;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Models;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeStatuses.Queries.Handlers
{
    public class GetCommitteeStatusesQueryHandler
         : IRequestHandler<GetCommitteeStatusesQuery, IEnumerable<GetCommitteeStatusResponse>>
    {

        #region Fields
        private readonly IAsyncRepository<CommitteeStatus> _statusRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor

        public GetCommitteeStatusesQueryHandler(IAsyncRepository<CommitteeStatus> statusRepository, IMapper mapper)
        {
            _statusRepository = statusRepository;
            _mapper = mapper;
        }

        #endregion

        #region Actions

        public async Task<IEnumerable<GetCommitteeStatusResponse>> Handle(GetCommitteeStatusesQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _statusRepository.GetAllNoTrackingAsync();
            return _mapper.Map<IEnumerable<GetCommitteeStatusResponse>>(statuses);
        }

        #endregion
    }
}
