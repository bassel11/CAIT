using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeApplication.Features.CommitteeMembers.Commands.Results;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Handlers
{
    public class RemoCommitMembCommandHandler
        : IRequestHandler<RemoveCommitteeMembersCommand, RemoveCommitteeMembersResult>
    {
        private readonly ICommitteeMemberRepository _repository;
        private readonly ILogger<RemoCommitMembCommandHandler> _logger;

        public RemoCommitMembCommandHandler(ICommitteeMemberRepository repository, ILogger<RemoCommitMembCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<RemoveCommitteeMembersResult> Handle(RemoveCommitteeMembersCommand request, CancellationToken cancellationToken)
        {
            var result = new RemoveCommitteeMembersResult();
            var membersToRemove = new List<CommitteeMember>();

            foreach (var memberId in request.MembersIds)
            {
                var member = await _repository.GetByCommitteeAndUserAsync(request.CommitteeId, memberId);
                if (member == null)
                {
                    result.NotFoundMemberIds.Add(memberId);
                    _logger.LogWarning($"Member {memberId} not found in committee {request.CommitteeId}");
                    continue;
                }

                membersToRemove.Add(member);
                result.RemovedMemberIds.Add(memberId);
            }

            if (membersToRemove.Any())
            {
                await _repository.RemoveRangeAsync(membersToRemove);
                _logger.LogInformation($"{membersToRemove.Count} members removed from committee {request.CommitteeId}");
            }

            return result;
        }
    }
}
