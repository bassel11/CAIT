using AutoMapper;
using CommitteeApplication.Extensions;
using CommitteeApplication.Features.StatusHistories.Queries.Models;
using CommitteeApplication.Features.StatusHistories.Queries.Results;
using CommitteeApplication.Resources;
using CommitteeApplication.Responses;
using CommitteeApplication.Wrappers;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CommitteeApplication.Features.StatusHistories.Queries.Handlers
{
    public class GetCommitStatusHistoryQueryHandler :
                   ResponseHandler
                 , IRequestHandler<GetCommitStatusHistoryQuery, PaginatedResult<CommitStatusHistoryResponse>>
    {

        #region Fields

        private readonly IStatusHistoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly IPaginationService _paginationService;

        #endregion

        #region Constructors

        public GetCommitStatusHistoryQueryHandler(IStatusHistoryRepository repository
                                                , IMapper mapper
                                                , IStringLocalizer<SharedResources> stringLocalizer
                                                , IPaginationService paginationService) : base(stringLocalizer)
        {
            _repository = repository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _paginationService = paginationService;

        }

        public async Task<PaginatedResult<CommitStatusHistoryResponse>> Handle(GetCommitStatusHistoryQuery request, CancellationToken cancellationToken)
        {
            // 🔹 Base query
            var query = _repository.Query();

            // 🔹 Filter by CommitteeId if provided
            if (request.CommitteeId.HasValue)  // request.CommitteeId != Guid.Empty

            {
                query = query.Where(h => h.CommitteeId == request.CommitteeId);
            }

            // 🔍 Search in DecisionText, OldStatus.Name, NewStatus.Name
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.ApplySearch(request.Search, h =>
                    h.DecisionText.Contains(request.Search) ||
                    h.OldStatus.Name.Contains(request.Search) ||
                    h.NewStatus.Name.Contains(request.Search));
            }

            // 🧩 Dynamic filters
            query = query.ApplyDynamicFilters(request.Filters);

            // ↕ Sorting
            query = query.ApplySorting(request.SortBy, defaultSort: "ChangedAt desc");

            // 🧯 Projection to DTO via AutoMapper (SQL JOIN)
            var projected = _mapper.ProjectTo<CommitStatusHistoryResponse>(query);

            // 📄 Pagination
            return await _paginationService.PaginateAsync(
                projected,
                request.PageNumber,
                request.PageSize
            );
        }

        #endregion

    }
}
