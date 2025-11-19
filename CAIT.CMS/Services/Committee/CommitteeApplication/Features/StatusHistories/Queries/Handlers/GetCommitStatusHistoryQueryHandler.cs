using AutoMapper;
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

        public Task<PaginatedResult<CommitStatusHistoryResponse>> Handle(GetCommitStatusHistoryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
