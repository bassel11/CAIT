using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Handlers
{
    public class DeleteQuorumRuleCommandHandler : IRequestHandler<DeleteQuorumRuleCommand, bool>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;

        public DeleteQuorumRuleCommandHandler(ICommitteeQuorumRuleRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(DeleteQuorumRuleCommand cmd, CancellationToken ct)
        {
            var rule = await _repo.GetByIdAsync(cmd.Id);
            if (rule == null) return false;

            await _repo.DeleteAsync(rule);
            return true;
        }
    }
}
