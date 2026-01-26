using Azure.Identity;
using IntegrationService.Application.Interfaces;
using IntegrationService.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace IntegrationService.Infrastructure.Services
{
    public class MicrosoftGraphService : IMeetingPlatformService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly AzureAdOptions _options;
        private readonly ILogger<MicrosoftGraphService> _logger;

        public MicrosoftGraphService(IOptions<AzureAdOptions> options, ILogger<MicrosoftGraphService> logger)
        {
            _options = options.Value;
            _logger = logger;

            // إعداد المصادقة باستخدام Client Secret (App-Only Auth)
            var clientSecretCredential = new ClientSecretCredential(
                _options.TenantId, _options.ClientId, _options.ClientSecret);

            _graphClient = new GraphServiceClient(clientSecretCredential);
        }

        public async Task<MeetingIntegrationResult> CreateOnlineMeetingAsync(
            string subject, string content, DateTime startUtc, DateTime endUtc, List<string> attendeeEmails)
        {
            try
            {
                var requestBody = new Event
                {
                    Subject = subject,
                    Body = new ItemBody { ContentType = BodyType.Html, Content = content },
                    Start = new DateTimeTimeZone { DateTime = startUtc.ToString("o"), TimeZone = "UTC" },
                    End = new DateTimeTimeZone { DateTime = endUtc.ToString("o"), TimeZone = "UTC" },

                    // ✅ تفعيل Teams ورابط Online
                    IsOnlineMeeting = true,
                    OnlineMeetingProvider = OnlineMeetingProviderType.TeamsForBusiness,

                    // إضافة الحضور
                    Attendees = attendeeEmails.Select(email => new Attendee
                    {
                        EmailAddress = new EmailAddress { Address = email },
                        Type = AttendeeType.Required
                    }).ToList(),

                    // خيار مهم: عدم السماح للمشاركين باقتراح وقت جديد (اختياري)
                    AllowNewTimeProposals = false
                };

                // إنشاء الاجتماع في تقويم "المنظم"
                var result = await _graphClient.Users[_options.OrganizerUserId]
                    .Calendar.Events
                    .PostAsync(requestBody);

                return new MeetingIntegrationResult(result.Id, result.OnlineMeeting?.JoinUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create meeting on Microsoft Graph for Subject: {Subject}", subject);
                throw; // مهم جداً رمي الخطأ لكي يقوم MassTransit بإعادة المحاولة
            }
        }


        public async Task CancelMeetingAsync(string outlookEventId, string cancellationReason)
        {
            if (string.IsNullOrEmpty(outlookEventId))
            {
                _logger.LogWarning("Cannot cancel meeting without OutlookEventId.");
                return;
            }

            try
            {
                // لحذف الاجتماع وإرسال إشعار للإلغاء للمشاركين
                await _graphClient.Users[_options.OrganizerUserId]
                    .Calendar.Events[outlookEventId]
                    .DeleteAsync(); // Graph API يرسل الإلغاء تلقائياً للمشاركين

                _logger.LogInformation("Cancelled meeting {EventId} successfully.", outlookEventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel meeting {EventId}", outlookEventId);
                // لا نرمي الخطأ إذا كان الاجتماع غير موجود أصلاً (404)
                if (!ex.Message.Contains("ErrorItemNotFound")) throw;
            }
        }

        public async Task UpdateMeetingTimeAsync(string outlookEventId, DateTime newStartUtc, DateTime newEndUtc)
        {
            if (string.IsNullOrEmpty(outlookEventId)) return;

            try
            {
                var updateBody = new Event
                {
                    Start = new DateTimeTimeZone { DateTime = newStartUtc.ToString("o"), TimeZone = "UTC" },
                    End = new DateTimeTimeZone { DateTime = newEndUtc.ToString("o"), TimeZone = "UTC" }
                };

                await _graphClient.Users[_options.OrganizerUserId]
                    .Calendar.Events[outlookEventId]
                    .PatchAsync(updateBody);

                _logger.LogInformation("Updated meeting time for {EventId}.", outlookEventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update meeting time {EventId}", outlookEventId);
                throw;
            }
        }
    }
}
