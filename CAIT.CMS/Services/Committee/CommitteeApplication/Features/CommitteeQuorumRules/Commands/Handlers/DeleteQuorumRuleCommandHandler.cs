using BuildingBlocks.Shared.Exceptions;
using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Handlers
{
    public class DeleteQuorumRuleCommandHandler : IRequestHandler<DeleteQuorumRuleCommand, bool>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;
        private readonly ILogger<DeleteQuorumRuleCommandHandler> _logger;

        public DeleteQuorumRuleCommandHandler(ICommitteeQuorumRuleRepository repo,
                                              ILogger<DeleteQuorumRuleCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteQuorumRuleCommand cmd, CancellationToken ct)
        {
            var rule = await _repo.GetByIdAsync(cmd.Id);
            if (rule == null)
            {
                // هذا يضمن أن الكنترولر سيرجع 404 Not Found تلقائياً عبر الـ Middleware
                throw new NotFoundException(nameof(CommitteeQuorumRule), cmd.Id);
            }

            await _repo.DeleteAsync(rule);
            _logger.LogInformation($"Quorum Rule with Id {cmd.Id} was deleted successfully.");

            return true;
        }
    }
}
