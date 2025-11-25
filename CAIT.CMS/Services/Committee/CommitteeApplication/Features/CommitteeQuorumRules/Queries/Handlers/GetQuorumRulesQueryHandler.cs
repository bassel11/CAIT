using AutoMapper;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Handlers
{
    public class GetQuorumRulesQueryHandler
        : IRequestHandler<GetQuorumRulesQuery, IEnumerable<CommitteeQuorumRuleResponse>>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;
        private readonly IMapper _mapper;

        public GetQuorumRulesQueryHandler(ICommitteeQuorumRuleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommitteeQuorumRuleResponse>> Handle(GetQuorumRulesQuery query, CancellationToken ct)
        {
            var rules = await _repo.GetAllAsync();

            var rulesListMapper = _mapper.Map<List<CommitteeQuorumRuleResponse>>(rules);
            return rulesListMapper;
        }
    }
}
