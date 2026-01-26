using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
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
            _logger.LogInformation("Integration Service: Received Schedule Request for Meeting {MeetingId}", msg.MeetingId);

            try
            {
                // 1. تحضير الإيميلات
                // ملاحظة: الحدث يحتوي على UserIds (Guid). في بيئة مثالية، نستعلم من Identity Service.
                // للتبسيط ولضمان العمل الآن، سنفترض وجود دالة مساعدة أو أننا عدلنا الحدث ليحمل الإيميلات.
                // سأضع هنا قائمة وهمية للتجربة، ويجب عليك لاحقاً استبدالها بـ Lookup حقيقي.
                var emails = await ResolveEmailsAsync(msg.AttendeeIds);

                // 2. التنفيذ الفعلي
                var result = await _platformService.CreateOnlineMeetingAsync(
                    msg.Title,
                    $"Committee Meeting Scheduled. ID: {msg.MeetingId}", // يمكن تحسين الوصف
                    msg.StartDate,
                    msg.EndDate,
                    emails
                );

                // 3. النجاح: إرسال الروابط إلى Meeting Service
                // يجب أن تكون قد عرفت هذا الحدث في BuildingBlocks.Contracts
                await _publishEndpoint.Publish(new MeetingPlatformCreatedIntegrationEvent
                {
                    MeetingId = msg.MeetingId,
                    OutlookEventId = result.OutlookEventId,
                    TeamsLink = result.TeamsJoinUrl
                });

                _logger.LogInformation("✅ Meeting Created on Teams/Outlook. Event ID: {EventId}", result.OutlookEventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Integration Failed for Meeting {MeetingId}", msg.MeetingId);
                throw; // Trigger Retry
            }
        }

        // دالة مساعدة (مؤقتة) لتحويل المعرفات لإيميلات
        private Task<List<string>> ResolveEmailsAsync(List<Guid> userIds)
        {
            // TODO: Call Identity Grpc Service here
            // return _identityGrpcService.GetEmailsAsync(userIds);
            return Task.FromResult(new List<string> { "member1@cait.gov.kw", "member2@cait.gov.kw" });
        }
    }
}
