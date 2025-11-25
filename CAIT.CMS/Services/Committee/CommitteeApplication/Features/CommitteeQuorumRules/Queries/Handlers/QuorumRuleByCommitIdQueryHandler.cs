using AutoMapper;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Handlers
{
    public class QuorumRuleByCommitIdQueryHandler
        : IRequestHandler<QuorumRuleByCommitIdQuery, CommitteeQuorumRuleResponse>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;
        private readonly IMapper _mapper;

        public QuorumRuleByCommitIdQueryHandler(ICommitteeQuorumRuleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CommitteeQuorumRuleResponse?> Handle(QuorumRuleByCommitIdQuery query, CancellationToken ct)
        {
            // استخدام التابع الجديد في الـ Repository
            var rule = await _repo.GetByCommitteeIdAsync(query.CommitteeId);
            if (rule == null) return null;

            // التحويل باستخدام AutoMapper
            var result = _mapper.Map<CommitteeQuorumRuleResponse>(rule);

            return result;

        }
    }
}
