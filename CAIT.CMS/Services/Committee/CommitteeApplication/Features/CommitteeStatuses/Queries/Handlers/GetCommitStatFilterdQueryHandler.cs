using AutoMapper;
using CommitteeApplication.Extensions;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Models;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using CommitteeApplication.Resources;
using CommitteeApplication.Responses;
using CommitteeApplication.Wrappers;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CommitteeApplication.Features.CommitteeStatuses.Queries.Handlers
{
    public class GetCommitStatFilterdQueryHandler : ResponseHandler
                , IRequestHandler<GetCommitStatFilterdQuery, PaginatedResult<GetCommitteeStatusResponse>>
    {
        #region Fields

        private readonly IAsyncRepository<CommitteeStatus> _repository;
        private readonly IMapper _mapper;
        private readonly IPaginationService _paginationService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        #endregion

        #region Constructors
        public GetCommitStatFilterdQueryHandler(
            IAsyncRepository<CommitteeStatus> repository,
            IMapper mapper,
            IPaginationService paginationService,
            IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _repository = repository;
            _mapper = mapper;
            _paginationService = paginationService;
            _stringLocalizer = stringLocalizer;
        }

        #endregion

        #region Actions

        public async Task<PaginatedResult<GetCommitteeStatusResponse>> Handle(GetCommitStatFilterdQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.Query();

            //  Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, c =>
                    (c.Name != null && c.Name.Contains(request.Search))
                );
            }

            //  Dynamic Filters
            query = query.ApplyDynamicFilters(request.Filters);

            //  Multi Sorting
            query = query.ApplySorting(request.SortBy, defaultSort: "CreatedAt desc");

            //  Projection
            var projected = _mapper.ProjectTo<GetCommitteeStatusResponse>(query);

            //  Pagination
            return await _paginationService.PaginateAsync(projected, request.PageNumber, request.PageSize);
        }

        #endregion
    }
}
