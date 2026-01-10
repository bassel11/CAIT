using AutoMapper;
using BuildingBlocks.Shared.Services;
using MediatR;
using MeetingApplication.Extensions;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingApplication.Resources;
using MeetingApplication.Responses;
using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using Microsoft.Extensions.Localization;

namespace MeetingApplication.Features.MoMs.Queries.Handlers
{
    public class GetMoMsByMeetingQueryHandler : ResponseHandler
                                              , IRequestHandler<GetMoMsByMeetingQuery, PaginatedResult<GetMinutesResponse>>
    {
        #region Fields
        private readonly IMoMRepository _momRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IPaginationService _paginationService;
        private readonly ICurrentUserService _currentUser;
        #endregion
        #region Constructor
        public GetMoMsByMeetingQueryHandler(IMoMRepository momRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer
                                    , IPaginationService paginationService
                                    , ICurrentUserService currentUser) : base(stringLocalizer)
        {
            _momRepository = momRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;
            _currentUser = currentUser;
        }
        #endregion

        public async Task<PaginatedResult<GetMinutesResponse>> Handle(GetMoMsByMeetingQuery request, CancellationToken cancellationToken)
        {
            var query = _momRepository.Query();

            if (request.MeetingId == Guid.Empty)
                query = _momRepository.Query()
                                  .Where(m => m.MeetingId == request.MeetingId);

            // 2) Search
            // ---------------------------------------------------------
            // 2) Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.AttendanceSummary != null && c.AttendanceSummary.Contains(request.Search)) ||
                    (c.AgendaSummary != null && c.AgendaSummary.Contains(request.Search)) ||
                    (c.DecisionsSummary != null && c.DecisionsSummary.Contains(request.Search))
                );
            }


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
            var projected = _mapper.ProjectTo<GetMinutesResponse>(query);

            // ---------------------------------------------------------
            // 6) Pagination
            // ---------------------------------------------------------
            return await _paginationService.PaginateAsync(
                projected,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}
