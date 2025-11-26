using MeetingApplication.Interfaces.Committee;
using MeetingCore.Enums;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MeetingInfrastructure.Integrations.Committee
{
    public class CommitteeServiceClient : ICommitteeService
    {
        private readonly HttpClient _http;
        private readonly ILogger<CommitteeServiceClient> _logger;

        public CommitteeServiceClient(
            HttpClient http,
            ILogger<CommitteeServiceClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<int> GetMemberCountAsync(Guid committeeId, CancellationToken ct = default)
        {
            var url = $"api/CommitteeMember/{committeeId}/members/count";

            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "CommitteeService: Failed to fetch member count for committee {CommitteeId}",
                    committeeId);

                return 0;
            }

            var data = await response.Content.ReadFromJsonAsync<MemberCountResponse>(
                cancellationToken: ct);

            return data?.ActiveMemberCount ?? 0;
        }

        public async Task<QuorumRule?> GetQuorumRuleAsync(Guid committeeId, CancellationToken ct = default)
        {
            var url = $"/api/CommitteeQuorumRule/GetByCommitteeId/{committeeId}";

            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "CommitteeService: No quorum rule for committee {CommitteeId}.",
                    committeeId);

                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<CommitteeQuorumRuleResponse>(
                cancellationToken: ct);

            if (data == null)
                return null;

            return new QuorumRule
            {
                Type = data.Type,
                Threshold = data.ThresholdPercent,
                UsePlusOne = data.UsePlusOne,
                AbsoluteCount = data.AbsoluteCount
            };
        }

        public class MemberCountResponse
        {
            public Guid CommitteeId { get; set; }
            public int ActiveMemberCount { get; set; }
        }

        public class CommitteeQuorumRuleResponse
        {
            public Guid Id { get; set; }
            public Guid CommitteeId { get; set; }
            public QuorumType Type { get; set; }
            public decimal? ThresholdPercent { get; set; }
            public int? AbsoluteCount { get; set; }
            public bool UsePlusOne { get; set; }
            public string? Description { get; set; }
            public DateTime EffectiveDate { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }
    }
}
