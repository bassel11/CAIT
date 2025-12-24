using BuildingBlocks.Contracts.Task.IntegrationEvents;
using MassTransit;
using MediatR;
using TaskCore.Events.AutomationEvents;

namespace TaskApplication.Features.Automation.EventHandlers
{
    public class EscalationNotificationHandler : INotificationHandler<TaskEscalatedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint; // واجهة النشر في MassTransit

        public EscalationNotificationHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(TaskEscalatedEvent notification, CancellationToken cancellationToken)
        {
            // تحويل Domain Event (الخاص بالمهام) -> Integration Event (العام للنظام)
            var integrationEvent = new TaskEscalatedIntegrationEvent
            {
                TaskId = notification.TaskId,
                TaskTitle = notification.TaskTitle,
                OriginalDeadline = notification.OriginalDeadline,
                DaysOverdue = notification.DaysOverdue,
                CommitteeId = notification.CommitteeId,
                AssigneeIds = notification.AssigneeIds
            };

            // إطلاق الحدث إلى وسيط الرسائل (RabbitMQ/Azure Service Bus)
            // خدمة المهام تنفض يدها هنا وتنتهي مسؤوليتها
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
    }
}
