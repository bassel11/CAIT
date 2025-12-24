using BuildingBlocks.Contracts.Task.IntegrationEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskCore.Events.AutomationEvents;

namespace TaskApplication.Features.Automation.EventHandlers
{
    // هذا هو الهاندلر المفقود الذي سيستمع لحدث "اقتراب الموعد"
    public class ReminderNotificationHandler : INotificationHandler<TaskDeadlineApproachingEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ReminderNotificationHandler> _logger;

        public ReminderNotificationHandler(IPublishEndpoint publishEndpoint, ILogger<ReminderNotificationHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(TaskDeadlineApproachingEvent notification, CancellationToken cancellationToken)
        {
            // تحويل من Domain Event إلى Integration Event
            var integrationEvent = new TaskReminderIntegrationEvent
            {
                TaskId = notification.TaskId,
                TaskTitle = notification.TaskTitle,
                Deadline = notification.Deadline,
                DaysRemaining = notification.DaysRemaining,
                AssigneeIds = notification.AssigneeIds
            };

            // إرسال لـ RabbitMQ
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);

            _logger.LogInformation("✅ Successfully published Reminder for Task: {TaskId}", notification.TaskId);
        }
    }
}
