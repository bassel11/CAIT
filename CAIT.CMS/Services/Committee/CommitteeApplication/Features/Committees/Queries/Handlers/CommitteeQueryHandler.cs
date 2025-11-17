using AutoMapper;
using CommitteeApplication.Extensions;
using CommitteeApplication.Features.Committees.Queries.Models;
using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Resources;
using CommitteeApplication.Responses;
using CommitteeApplication.Wrappers;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CommitteeApplication.Features.Committees.Queries.Handlers
{
    public class CommitteeQueryHandler : ResponseHandler
                                        , IRequestHandler<GetCommitteeByIdQuery, Response<GetCommitteeByIdResponse>>
                                        , IRequestHandler<GetCommitteeListQuery, Response<List<GetCommitteeListResponse>>>
                                        , IRequestHandler<GetComitsFilteredQuery, PaginatedResult<GetComitsFilteredResponse>>
    {
        #region Fields
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IPaginationService _paginationService;
        #endregion


        #region Constructor
        public CommitteeQueryHandler(ICommitteeRepository committeeRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer
                                    , IPaginationService paginationService) : base(stringLocalizer)
        {
            _committeeRepository = committeeRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;
        }
        #endregion


        #region Actions
        public async Task<Response<GetCommitteeByIdResponse>> Handle(GetCommitteeByIdQuery request, CancellationToken cancellationToken)
        {
            var committeeList = await _committeeRepository.GetByIdAsync(request.Id);

            if (committeeList == null)
                return NotFound<GetCommitteeByIdResponse>(
                    _stringLocalizer[SharedResourcesKeys.NotFound]);

            var result = _mapper.Map<GetCommitteeByIdResponse>(committeeList);

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

        public async Task<PaginatedResult<GetComitsFilteredResponse>> Handle(GetComitsFilteredQuery request, CancellationToken cancellationToken)
        {
            var query = _committeeRepository.Query(); // ← هنا

            // 🔍 Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.Name != null && c.Name.Contains(request.Search)) ||
                    (c.Scope != null && c.Scope.Contains(request.Search)) ||
                    (c.Purpose != null && c.Purpose.Contains(request.Search))
                );
            }


            // 🧩 Dynamic Filters
            query = query.ApplyDynamicFilters(request.Filters);

            // ↕ Multi Sorting
            query = query.ApplySorting(request.SortBy);

            // 🧯 Projection
            var projected = _mapper.ProjectTo<GetComitsFilteredResponse>(query);

            // 📄 Pagination
            return await _paginationService.PaginateAsync(projected, request.PageNumber, request.PageSize);
        }

        #endregion
    }
}
