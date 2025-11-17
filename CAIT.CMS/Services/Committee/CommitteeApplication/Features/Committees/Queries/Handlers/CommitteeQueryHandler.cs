using AutoMapper;
using CommitteeApplication.Features.Committees.Queries.Models;
using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Resources;
using CommitteeApplication.Responses;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CommitteeApplication.Features.Committees.Queries.Handlers
{
    public class CommitteeQueryHandler : ResponseHandler
                                        , IRequestHandler<GetCommitteeByIdQuery, Response<List<GetCommitteeByIdResponse>>>
                                        , IRequestHandler<GetCommitteeListQuery, Response<List<GetCommitteeListResponse>>>
    {
        #region Fields
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion


        #region Constructor
        public CommitteeQueryHandler(ICommitteeRepository committeeRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _committeeRepository = committeeRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
        }
        #endregion


        #region Actions
        public async Task<Response<List<GetCommitteeByIdResponse>>> Handle(GetCommitteeByIdQuery request, CancellationToken cancellationToken)
        {
            var committeeList = await _committeeRepository.GetByIdAsync(request.Id);

            if (committeeList == null)
                return NotFound<List<GetCommitteeByIdResponse>>(
                    _stringLocalizer[SharedResourcesKeys.NotFound]);

            var result = _mapper.Map<List<GetCommitteeByIdResponse>>(committeeList);

            return Success(result);
        }

        public async Task<Response<List<GetCommitteeListResponse>>> Handle(GetCommitteeListQuery request, CancellationToken cancellationToken)
        {
            var committeeList = await _committeeRepository
                .GetAllNoTrackingAsync();

            var committeeListMapper = _mapper.Map<List<GetCommitteeListResponse>>(committeeList);
            var result = Success(committeeListMapper);
            return result;
        }

        #endregion
    }
}
