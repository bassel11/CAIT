using AutoMapper;
using CommitteeApplication.Extensions;
using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeApplication.Interfaces.Grpc;
using CommitteeApplication.Resources;
using CommitteeApplication.Responses;
using CommitteeApplication.Wrappers;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Handlers
{
    public class GetComitMembsFilteredQueryHandler : ResponseHandler
                , IRequestHandler<GetComitMembsFilteredQuery, PaginatedResult<CommitMembsFilterResponse>>
    {

        #region Fields

        private readonly ICommitteeMemberRepository _commitMembRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IPaginationService _paginationService;
        private readonly IUserGrpcService _userGrpcService;

        #endregion

        #region Constructor
        public GetComitMembsFilteredQueryHandler(ICommitteeMemberRepository commitMembRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer
                                    , IPaginationService paginationService
                                    , IUserGrpcService userGrpcService) : base(stringLocalizer)
        {
            _commitMembRepository = commitMembRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;
            _userGrpcService = userGrpcService;
        }
        #endregion

        #region Actions
        public async Task<PaginatedResult<CommitMembsFilterResponse>> Handle(GetComitMembsFilteredQuery request, CancellationToken cancellationToken)
        {

            var query = _commitMembRepository.Query();

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.ContactDetails != null && c.ContactDetails.Contains(request.Search)) ||
                    (c.Affiliation != null && c.Affiliation.Contains(request.Search))
                );
            }

            // Filters
            query = query.ApplyDynamicFilters(request.Filters);

            // Sorting
            query = query.ApplySorting(request.SortBy, defaultSort: "CreatedAt desc");

            // Bring entities from database (NOW allowed)
            var members = query.ToList(); // NOT ToListAsync — this is Application Layer

            // Extract userIds
            var userIds = members
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

            // gRPC Bulk Request
            var users = await _userGrpcService.GetUsersByIdsAsync(userIds);

            var dict = users.ToDictionary(u => u.UserId, u => u);

            // Map to DTO and merge only FirstName, LastName, Email
            var list = members.Select(m =>
            {
                var dto = _mapper.Map<CommitMembsFilterResponse>(m);

                if (dict.TryGetValue(m.UserId, out var user))
                {
                    dto.FullNAME = user.FirstName;
                    dto.UserEmail = user.Email;
                }

                return dto;
            }).ToList();

            return await _paginationService.PaginateListAsync(list, request.PageNumber, request.PageSize);


        }


        //public async Task<PaginatedResult<CommitMembsFilterResponse>> Handle(GetComitMembsFilteredQuery request, CancellationToken cancellationToken)
        //{
        //    var query = _commitMembRepository.Query(); // ← هنا

        //    Guid id = Guid.Parse("5D51A287-EFE8-49AB-C46D-08DE113C9D22");

        //    var users = await _userGrpcService.GetUserByIdAsync(id);

        //    // 🔍 Search
        //    if (!string.IsNullOrWhiteSpace(request.Search))
        //    {
        //        query = query.ApplySearch(request.Search, c =>
        //            (c.ContactDetails != null && c.ContactDetails.Contains(request.Search)) ||
        //            (c.Affiliation != null && c.Affiliation.Contains(request.Search))
        //        );
        //    }


        //    // 🧩 Dynamic Filters
        //    query = query.ApplyDynamicFilters(request.Filters);

        //    // ↕ Multi Sorting
        //    query = query.ApplySorting(request.SortBy, defaultSort: "CreatedAt desc");

        //    // 🧯 Projection
        //    var projected = _mapper.ProjectTo<CommitMembsFilterResponse>(query);

        //    // 📄 Pagination
        //    return await _paginationService.PaginateAsync(projected, request.PageNumber, request.PageSize);
        //}

        #endregion
    }
}
