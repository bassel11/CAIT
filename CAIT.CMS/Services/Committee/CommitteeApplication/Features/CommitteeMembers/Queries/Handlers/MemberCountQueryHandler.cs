using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Handlers
{
    public class MemberCountQueryHandler
        : IRequestHandler<MemberCountQuery, MemberCountResponse>
    {
        private readonly ICommitteeMemberRepository _committeeMemberRepo;
        private readonly ICommitteeRepository _commiteeRepository;
        private readonly ILogger<MemberCountQueryHandler> _logger;

        public MemberCountQueryHandler(
            ICommitteeMemberRepository committeeMemberRepo,
            ICommitteeRepository commiteeRepository,
            ILogger<MemberCountQueryHandler> logger)
        {
            _committeeMemberRepo = committeeMemberRepo;
            _logger = logger;
            _commiteeRepository = commiteeRepository;
        }

        public async Task<MemberCountResponse> Handle(MemberCountQuery request, CancellationToken ct)
        {
            //var cacheKey = $"committee:{request.CommitteeId}:membercount";

            //var count = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            //{
            //    entry.AbsoluteExpirationRelativeToNow = _ttl;
            //    var c = await _repo.GetActiveMemberCountAsync(request.CommitteeId, ct);
            //    _logger.LogDebug("Fetched member count for {CommitteeId}: {Count}", request.CommitteeId, c);
            //    return c;
            //});

            if (request.CommitteeId == Guid.Empty)
            {
                _logger.LogWarning("Invalid CommitteeId passed to MemberCountQuery.");
                throw new ArgumentException("CommitteeId must not be empty.", nameof(request.CommitteeId));
            }

            // تحقق من وجود اللجنة نفسها
            var committee = await _commiteeRepository.GetByIdAsync(request.CommitteeId);
            if (committee == null)
            {
                _logger.LogWarning("Committee with Id {CommitteeId} was not found.", request.CommitteeId);
                throw new KeyNotFoundException($"Committee with Id {request.CommitteeId} not found.");
            }

            var count = await _committeeMemberRepo.GetActiveMemberCountAsync(request.CommitteeId, ct);

            return new MemberCountResponse
            {
                CommitteeId = request.CommitteeId,
                ActiveMemberCount = count
            };
        }
    }
}
