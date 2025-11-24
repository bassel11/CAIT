using AutoMapper;
using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Extensions;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Resources;
using MeetingApplication.Responses;
using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using Microsoft.Extensions.Localization;

namespace MeetingApplication.Features.Meetings.Queries.Handlers
{
    public class GetMeetingsQueryHandler : ResponseHandler
                                         , IRequestHandler<GetMeetingsQuery, PaginatedResult<GetMeetingResponse>>
    {
        #region Fields
        private readonly IMeetingRepository _metingRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IPaginationService _paginationService;
        private readonly ICurrentUserService _currentUser;
        #endregion
        #region Constructor
        public GetMeetingsQueryHandler(IMeetingRepository meetingRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer
                                    , IPaginationService paginationService
                                    , ICurrentUserService currentUser) : base(stringLocalizer)
        {
            _metingRepository = meetingRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;
            _currentUser = currentUser;
        }
        #endregion

        public async Task<PaginatedResult<GetMeetingResponse>> Handle(GetMeetingsQuery request, CancellationToken cancellationToken)
        {
            var query = _metingRepository.Query();


            // 2) Search
            // ---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.Title != null && c.Title.Contains(request.Search)) ||
                    (c.Description != null && c.Description.Contains(request.Search))
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
            var projected = _mapper.ProjectTo<GetMeetingResponse>(query);

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
