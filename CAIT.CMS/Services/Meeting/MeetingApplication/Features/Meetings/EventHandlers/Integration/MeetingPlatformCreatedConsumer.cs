using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using MassTransit;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Integration
{
    public class MeetingPlatformCreatedConsumer : IConsumer<MeetingPlatformCreatedIntegrationEvent>
    {
        private readonly IMeetingRepository _repository;
        private readonly ILogger<MeetingPlatformCreatedConsumer> _logger;

        public MeetingPlatformCreatedConsumer(
            IMeetingRepository repository,
            ILogger<MeetingPlatformCreatedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MeetingPlatformCreatedIntegrationEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation("📥 Meeting Service: Received Integration Result for Meeting {MeetingId}", msg.MeetingId);

            // 1. جلب الاجتماع (نحتاج الـ Aggregate Root لتحديثه)
            var meetingId = MeetingId.Of(msg.MeetingId);
            var meeting = await _repository.GetByIdAsync(meetingId, context.CancellationToken);

            if (meeting == null)
            {
                _logger.LogError("❌ Meeting not found for ID: {MeetingId}. Cannot update integration info.", msg.MeetingId);
                return;
            }

            // 2. تحديث البيانات باستخدام الدالة الموجودة في الدومين
            // هذا يحقق المتطلب: "invitations automatically sync... secure Microsoft Teams link is auto-generated and embedded"
            meeting.UpdateIntegrationInfo(msg.OutlookEventId, msg.TeamsLink);

            // 3. الحفظ
            await _repository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ Meeting Updated successfully with Teams Link and Outlook ID.");
        }
    }
}
