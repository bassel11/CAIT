using AutoMapper;
using CommitteeApplication.Common.CurrentUser;
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
        private readonly ICurrentUserService _currentUser;
        #endregion


        #region Constructor
        public CommitteeQueryHandler(ICommitteeRepository committeeRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer
                                    , IPaginationService paginationService
                                    , ICurrentUserService currentUser) : base(stringLocalizer)
        {
            _committeeRepository = committeeRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;
            _currentUser = currentUser;
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
            var query = _committeeRepository.Query();

            // ---------------------------------------------------------
            // 1) SuperAdmin → يشاهد جميع اللجان بدون قيود
            // ---------------------------------------------------------
            if (!_currentUser.IsSuperAdmin)
            {
                var userId = _currentUser.UserId;

                // انضمام بسيط للحصول على اللجان التي هو عضو فيها فقط
                query = query
                    .Where(c => c.CommitteeMembers.Any(m => m.UserId == userId));
            }

            // ---------------------------------------------------------
            // 2) Search
            // ---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.Name != null && c.Name.Contains(request.Search)) ||
                    (c.Scope != null && c.Scope.Contains(request.Search)) ||
                    (c.Purpose != null && c.Purpose.Contains(request.Search))
                );
            }

            // ---------------------------------------------------------
            // 3) Dynamic Filters
            // ---------------------------------------------------------
            query = query.ApplyDynamicFilters(request.Filters);

            // ---------------------------------------------------------
            // 4) Sorting
            // ---------------------------------------------------------
            query = query.ApplySorting(request.SortBy, defaultSort: "CreatedAt desc");

            // ---------------------------------------------------------
            // 5) Projection (AutoMapper → يُحوّل الاستعلام لـ SELECT فقط)
            // ---------------------------------------------------------
            var projected = _mapper.ProjectTo<GetComitsFilteredResponse>(query);

            // ---------------------------------------------------------
            // 6) Pagination
            // ---------------------------------------------------------
            return await _paginationService.PaginateAsync(
                projected,
                request.PageNumber,
                request.PageSize
            );
        }

        #endregion
    }
}
