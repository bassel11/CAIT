//using AutoMapper;
//using BuildingBlocks.Shared.Services;
//using MediatR;
//using MeetingApplication.Extensions;
//using MeetingApplication.Features.Attendances.Queries.Models;
//using MeetingApplication.Features.Attendances.Queries.Results;
//using MeetingApplication.Resources;
//using MeetingApplication.Responses;
//using MeetingApplication.Wrappers;
//using MeetingCore.Repositories;
//using Microsoft.Extensions.Localization;

//namespace MeetingApplication.Features.Attendances.Queries.Handlers
//{
//    public class GetAttendanceForMemberQueryHandler : ResponseHandler
//                                                    , IRequestHandler<GetAttendanceForMemberQuery, PaginatedResult<AttendanceResponse>>
//    {
//        #region Fields
//        private readonly IAttendanceRepository _attendanceRepository;
//        private readonly IMapper _mapper;
//        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
//        private readonly IPaginationService _paginationService;
//        private readonly ICurrentUserService _currentUser;
//        #endregion
//        #region Constructor
//        public GetAttendanceForMemberQueryHandler(IAttendanceRepository attendanceRepository
//                                    , IMapper mapper
//                                    , IStringLocalizer<SharedResources> stringLocalizer
//                                    , IPaginationService paginationService
//                                    , ICurrentUserService currentUser) : base(stringLocalizer)
//        {
//            _attendanceRepository = attendanceRepository;
//            _mapper = mapper;
//            _stringLocalizer = stringLocalizer;
//            _paginationService = paginationService;
//            _currentUser = currentUser;
//        }
//        #endregion

//        public async Task<PaginatedResult<AttendanceResponse>> Handle(GetAttendanceForMemberQuery request, CancellationToken cancellationToken)
//        {
//            var query = _attendanceRepository.Query();


//            // 2) Search
//            // ---------------------------------------------------------
//            //if (!string.IsNullOrWhiteSpace(request.Search))
//            //{
//            //    query = query.ApplySearch(request.Search, c =>
//            //        (c.Title != null && c.Title.Contains(request.Search)) ||
//            //        (c.Description != null && c.Description.Contains(request.Search))
//            //    );
//            //}

//            // 3) Dynamic Filters
//            // ---------------------------------------------------------
//            query = query.ApplyDynamicFilters(request.Filters);

//            // ---------------------------------------------------------
//            // 4) Sorting
//            // ---------------------------------------------------------
//            query = query.ApplySorting(request.SortBy, defaultSort: "Timestamp desc");

//            // ---------------------------------------------------------
//            // 5) Projection (AutoMapper → يُحوّل الاستعلام لـ SELECT فقط)
//            // ---------------------------------------------------------
//            var projected = _mapper.ProjectTo<AttendanceResponse>(query);

//            // ---------------------------------------------------------
//            // 6) Pagination
//            // ---------------------------------------------------------
//            return await _paginationService.PaginateAsync(
//                projected,
//                request.PageNumber,
//                request.PageSize
//            );
//        }
//    }
//}
