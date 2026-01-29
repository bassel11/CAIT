using MeetingApplication.Interfaces.Committee;
using MeetingCore.Enums;
using MeetingCore.Enums.AttendanceEnums;
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

            try
            {
                var response = await _http.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("No quorum rule found for committee {Id}. Using default.", committeeId);
                    return null;
                }

                var data = await response.Content.ReadFromJsonAsync<CommitteeQuorumRuleResponse>(cancellationToken: ct);

                if (data == null) return null;

                // Mapping
                return new QuorumRule
                {
                    Type = (QuorumType)data.Type, // Ensure Enums match or map manually
                    ThresholdPercent = data.ThresholdPercent,
                    UsePlusOne = data.UsePlusOne,
                    AbsoluteCount = data.AbsoluteCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching quorum rule.");
                return null;
            }

        }

        public async Task<List<CommitteeMemberDto>> GetActiveMembersAsync(Guid committeeId, CancellationToken ct = default)
        {
            // المسار الجديد الذي أنشأناه في CommitteeController
            // تأكد أن المسار يطابق Route الكونترولر هناك (api/v1/CommitteeMember/GetCommitteeMembers/{id})
            var url = $"api/v1/CommitteeMember/GetCommitteeMembers/{committeeId}";

            try
            {
                var response = await _http.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("CommitteeService: Failed to fetch active members for committee {CommitteeId}. Status: {Status}",
                        committeeId, response.StatusCode);
                    return new List<CommitteeMemberDto>();
                }

                // 1. استقبال البيانات بالشكل القادم من الـ API (Raw Data)
                var apiResult = await response.Content.ReadFromJsonAsync<List<CommitteeMemberIntegrationResponse>>(cancellationToken: ct);

                if (apiResult == null || !apiResult.Any())
                    return new List<CommitteeMemberDto>();

                // 2. التحويل إلى DTO الخاص بالاجتماعات (Business Logic Mapping)
                return apiResult.Select(m => new CommitteeMemberDto
                {
                    UserId = m.UserId,
                    FullName = m.FullName,
                    Email = m.Email,

                    // تحويل الأرقام القادمة من اللجنة إلى مفاهيم الاجتماع
                    Role = MapCommitteeRoleToAttendanceRole(m.CommitteeRoleId),
                    VotingRight = m.HasVotingRight ? VotingRight.Voting : VotingRight.NonVoting

                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception fetching members for committee {CommitteeId}", committeeId);
                return new List<CommitteeMemberDto>();
            }
        }

        // ============================================================
        // Helper Classes & Methods (Internal)
        // ============================================================

        // كلاس داخلي يطابق الـ JSON القادم من خدمة اللجان
        private class CommitteeMemberIntegrationResponse
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public int CommitteeRoleId { get; set; }
            public bool HasVotingRight { get; set; }
        }

        // منطق التحويل (Mapping Logic)
        private AttendanceRole MapCommitteeRoleToAttendanceRole(int roleId)
        {
            // افترضنا بناءً على تحليلنا السابق:
            // 1=Chairman, 2=Vice, 3=Rapporteur, 4=Member, 5=Observer
            return roleId switch
            {
                1 or 2 or 3 => AttendanceRole.Required, // المناصب القيادية حضورها إلزامي
                4 => AttendanceRole.Optional,           // الأعضاء العاديون (حسب سياسة اللجنة)
                5 => AttendanceRole.Observer,           // المراقب
                _ => AttendanceRole.Optional
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
