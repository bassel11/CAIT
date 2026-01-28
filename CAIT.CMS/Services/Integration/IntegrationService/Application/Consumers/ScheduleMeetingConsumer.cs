using BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents;
using IntegrationService.Application.Interfaces;
using MassTransit;

namespace IntegrationService.Application.Consumers
{
    public class ScheduleMeetingConsumer : IConsumer<MeetingScheduledIntegrationEvent>
    {
        private readonly IMeetingPlatformService _platformService;
        private readonly ILogger<ScheduleMeetingConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public ScheduleMeetingConsumer(
            IMeetingPlatformService platformService,
            ILogger<ScheduleMeetingConsumer> logger,
            IPublishEndpoint publishEndpoint)
        {
            _platformService = platformService;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<MeetingScheduledIntegrationEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation("Integration Service: Processing Schedule Request for Meeting {MeetingId}", msg.MeetingId);

            try
            {
                // 1. استخراج الإيميلات (افترضنا وجود هذه الدالة المساعدة سابقاً)
                var emails = await ResolveEmailsAsync(msg.AttendeeIds);

                // 2. ✅ التحقق من التوفر (Conflict Check)
                // هل الأوقات متاحة في Outlook؟
                bool isAvailable = await _platformService.AreAttendeesAvailableAsync(
                    emails,
                    msg.StartDate,
                    msg.EndDate,
                    "UTC");

                // 3. 🛑 معالجة التعارض (Failure Path)
                if (!isAvailable)
                {
                    _logger.LogWarning("⚠️ Conflict Detected for Meeting {MeetingId}. Aborting creation.", msg.MeetingId);

                    // نشر حدث الفشل لإبلاغ الـ Meeting Service
                    await _publishEndpoint.Publish(new MeetingSchedulingFailedIntegrationEvent
                    {
                        MeetingId = msg.MeetingId,
                        Reason = "Conflict detected in attendees' Outlook calendars."
                    });

                    // نتوقف هنا ولا ننشئ الاجتماع
                    return;
                }

                // 4. ✅ المسار الناجح (Success Path)
                var result = await _platformService.CreateOnlineMeetingAsync(
                    msg.Title,
                    $"Committee Meeting. ID: {msg.MeetingId}",
                    msg.StartDate,
                    msg.EndDate,
                    emails
                );

                // إرسال نتيجة النجاح
                await _publishEndpoint.Publish(new MeetingPlatformCreatedIntegrationEvent
                {
                    MeetingId = msg.MeetingId,
                    OutlookEventId = result.OutlookEventId,
                    TeamsLink = result.TeamsJoinUrl
                });

                _logger.LogInformation("✅ Meeting Created on Teams successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected Error via Graph API for Meeting {MeetingId}", msg.MeetingId);

                // في حالة وجود خطأ تقني (وليس تعارض)، نرمي الخطأ ليعيد MassTransit المحاولة
                // أو يمكننا إرسال حدث فشل تقني أيضاً
                throw;
            }
        }

        // Mock method for emails
        private Task<List<string>> ResolveEmailsAsync(List<Guid> userIds)
        {
            // في الواقع يتم جلبه من Identity
            return Task.FromResult(new List<string> { "member1@cait.gov.kw", "member2@cait.gov.kw" });
        }
    }
}