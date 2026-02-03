using BuildingBlocks.Contracts.Meeting.MoMs.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.MoMEvents;

namespace MeetingApplication.Features.MoMs.EventHandlers.Domain
{
    public class MoMApprovedEventHandler : INotificationHandler<MoMApprovedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        // نحقن MassTransit IPublishEndpoint
        public MoMApprovedEventHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(MoMApprovedEvent notification, CancellationToken cancellationToken)
        {
            // 1. تحويل البيانات (Mapping)
            // من لغة الدومين الداخلية إلى لغة التكامل الخارجية

            var integrationEvent = new MoMApprovedIntegrationEvent
            {
                MeetingId = notification.MeetingId.Value,
                MoMId = notification.MoMId.Value,
                OccurredOn = notification.ApprovedAt,

                // تحويل القرارات
                Decisions = notification.Decisions.Select(d => new DecisionItemDto(
                    d.Title,
                    d.Content
                )).ToList(),

                // تحويل المهام
                Tasks = notification.Tasks.Select(t => new TaskItemDto(
                    t.Title,
                    t.AssigneeId,
                    t.DueDate
                )).ToList()
            };

            // 2. النشر (Publishing via Outbox)
            // ملاحظة مهمة جداً:
            // بما أنك قمت بإعداد Transactional Outbox في الـ DbContext
            // فإن هذا السطر لن يرسل الرسالة فوراً للوسيط (Broker)
            // بل سيقوم بحفظها في جدول OutboxMessages في نفس الترانزاكشن الخاصة بحفظ المحضر.
            // هذا يضمن (Atomicity) 100%.

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
    }
}
