using BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingApplication.Interfaces.Scheduling;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingScheduledEventHandler : INotificationHandler<MeetingScheduledEvent>
    {
        private readonly IMeetingSchedulerGateway _schedulerGateway; // Quartz wrapper
        private readonly IPublishEndpoint _publishEndpoint; // MassTransit
        private readonly ILogger<MeetingScheduledEventHandler> _logger;

        public MeetingScheduledEventHandler(
            IMeetingSchedulerGateway schedulerGateway,
            IPublishEndpoint publishEndpoint,
            ILogger<MeetingScheduledEventHandler> logger)
        {
            _schedulerGateway = schedulerGateway;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingScheduledEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingScheduledEvent));
            _logger.LogInformation("📢 Processing Domain Event: Meeting {Id} Scheduled", domainEvent.MeetingId);


            // 1. جدولة التذكيرات (Quartz) - Internal Concern
            await _schedulerGateway.ScheduleMeetingRemindersAsync(
                domainEvent.MeetingId.Value,
                domainEvent.Title,
                domainEvent.StartDate,
                cancellationToken
            );

            // 2. إبلاغ خدمة التكامل (MassTransit) - External Concern
            // لإنشاء الاجتماع على Teams/Outlook
            await _publishEndpoint.Publish(new MeetingScheduledIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId.Value,
                Title = domainEvent.Title,
                StartDate = domainEvent.StartDate,
                EndDate = domainEvent.EndDate,
                AttendeeIds = domainEvent.AttendeeIds.ToList(),
            }, cancellationToken);


            //var integrationEvent = new MeetingScheduledIntegrationEvent
            //{
            //    MeetingId = domainEvent.MeetingId.Value,
            //    CommitteeId = domainEvent.CommitteeId.Value,
            //    Title = domainEvent.Title,
            //    StartDate = domainEvent.StartDate,
            //    EndDate = domainEvent.EndDate,
            //    // تحويل IReadOnlyList<Guid> إلى List<Guid> لـ MassTransit
            //    AttendeeIds = domainEvent.AttendeeIds.ToList()
            //};

            //await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingScheduledIntegrationEvent));
        }
    }
}
