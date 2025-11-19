using AutoMapper;
using CommitteeApplication.Extensions;
using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
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

        #region

        private readonly ICommitteeMemberRepository _commitMembRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IPaginationService _paginationService;

        #endregion

        #region Constructor
        public GetComitMembsFilteredQueryHandler(ICommitteeMemberRepository commitMembRepository
                                    , IMapper mapper
                                    , IStringLocalizer<SharedResources> stringLocalizer
                                    , IPaginationService paginationService) : base(stringLocalizer)
        {
            _commitMembRepository = commitMembRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;
        }
        #endregion

        #region Actions
        public async Task<PaginatedResult<CommitMembsFilterResponse>> Handle(GetComitMembsFilteredQuery request, CancellationToken cancellationToken)
        {
            var query = _commitMembRepository.Query(); // ← هنا

            // 🔍 Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.ContactDetails != null && c.ContactDetails.Contains(request.Search)) ||
                    (c.Affiliation != null && c.Affiliation.Contains(request.Search))
                );
            }


            // 🧩 Dynamic Filters
            query = query.ApplyDynamicFilters(request.Filters);

            // ↕ Multi Sorting
            query = query.ApplySorting(request.SortBy);

            // 🧯 Projection
            var projected = _mapper.ProjectTo<CommitMembsFilterResponse>(query);

            // 📄 Pagination
            return await _paginationService.PaginateAsync(projected, request.PageNumber, request.PageSize);
        }

        #endregion
    }
}
