using AutoMapper;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Handlers
{
    public class GetQuorumRuleByIdQueryHandler : IRequestHandler<GetQuorumRuleByIdQuery, CommitteeQuorumRuleResponse>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;
        private readonly IMapper _mapper;

        public GetQuorumRuleByIdQueryHandler(ICommitteeQuorumRuleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CommitteeQuorumRuleResponse?> Handle(GetQuorumRuleByIdQuery query, CancellationToken ct)
        {
            var rule = await _repo.GetByIdAsync(query.Id);
            if (rule == null) return null;

            var result = _mapper.Map<CommitteeQuorumRuleResponse>(rule);

            return result;
        }
    }
}
