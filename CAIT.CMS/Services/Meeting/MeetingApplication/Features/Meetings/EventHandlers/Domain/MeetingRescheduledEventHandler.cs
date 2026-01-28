using BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingApplication.Interfaces.Scheduling;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingRescheduledEventHandler : INotificationHandler<MeetingRescheduledEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingRescheduledEventHandler> _logger;
        private readonly IMeetingSchedulerGateway _schedulerGateway;

        public MeetingRescheduledEventHandler(
            IPublishEndpoint publishEndpoint,
            ILogger<MeetingRescheduledEventHandler> logger,
            IMeetingSchedulerGateway schedulerGateway)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _schedulerGateway = schedulerGateway;
        }

        public async Task Handle(MeetingRescheduledEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingRescheduledEvent));


            // 1. تحديث التذكيرات (إلغاء القديم وإنشاء جديد)
            await _schedulerGateway.CancelMeetingRemindersAsync(
                domainEvent.MeetingId.Value,
                cancellationToken);

            // نحتاج لعنوان الاجتماع هنا، يمكن جلبه من الريبوزيتوري أو تمريره في الحدث
            // للتبسيط سنفترض وجوده أو جلبه
            await _schedulerGateway.ScheduleMeetingRemindersAsync(
               domainEvent.MeetingId.Value,
               "Rescheduled Meeting", // يفضل تمرير العنوان في الحدث
               domainEvent.NewStartDate,
               cancellationToken);


            // 2. إبلاغ التكامل للتحديث في Outlook
            var integrationEvent = new MeetingRescheduledIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId.Value,
                NewStartDate = domainEvent.NewStartDate,
                NewEndDate = domainEvent.NewEndDate,
                OutlookEventId = domainEvent.OutlookEventId
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingRescheduledIntegrationEvent));
        }
    }
}
