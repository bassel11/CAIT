using BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents;
using MassTransit;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Integration
{
    public class MeetingSchedulingFailedConsumer : IConsumer<MeetingSchedulingFailedIntegrationEvent>
    {
        private readonly IMeetingRepository _repository;
        private readonly ILogger<MeetingSchedulingFailedConsumer> _logger;
        // private readonly INotificationService _notificationService; // مستقبلاً لإشعار المستخدم

        public MeetingSchedulingFailedConsumer(
            IMeetingRepository repository,
            ILogger<MeetingSchedulingFailedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MeetingSchedulingFailedIntegrationEvent> context)
        {
            var msg = context.Message;
            _logger.LogWarning("📥 Meeting Service: Received Failure Alert for Meeting {MeetingId}. Reason: {Reason}", msg.MeetingId, msg.Reason);

            var meetingId = MeetingId.Of(msg.MeetingId);

            // 1. جلب الاجتماع مع الـ Tracking للتعديل
            var meeting = await _repository.GetByIdAsync(meetingId, context.CancellationToken);

            if (meeting == null)
            {
                _logger.LogError("Meeting {Id} not found during failure compensation.", msg.MeetingId);
                return;
            }

            // 2. تطبيق منطق التعويض في الدومين
            meeting.HandleSchedulingFailure(msg.Reason);

            // 3. حفظ التغييرات (Revert Status to Draft)
            // هذا سيؤدي تلقائياً لإلغاء أي Quartz Jobs مرتبطة إذا كان لديك Event Handler يستمع لتغير الحالة لـ Draft (اختياري)
            // أو يمكننا يدوياً طلب إلغاء الجدولة هنا إذا لم نكن نثق بالـ Events

            await _repository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ Meeting {Id} reverted to Draft status due to external conflict.", msg.MeetingId);

            // TODO: هنا يمكن إرسال إشعار (SignalR / Email) للمقرر: "تعذر حجز الاجتماع لوجود تعارض"
        }
    }
}
