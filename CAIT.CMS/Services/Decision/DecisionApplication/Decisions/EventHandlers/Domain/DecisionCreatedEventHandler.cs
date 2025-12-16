using MassTransit;
using Microsoft.FeatureManagement;

namespace DecisionApplication.Decisions.EventHandlers.Domain
{
    public class DecisionCreatedEventHandler : INotificationHandler<DecisionCreatedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint; // سنحتفظ بهذا الحقل كما هو
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<DecisionCreatedEventHandler> _logger;


        // 👇 التغيير هنا في الـ Constructor 👇
        public DecisionCreatedEventHandler(
            IPublishEndpoint publishEndpoint,
            IFeatureManager featureManager,
            ILogger<DecisionCreatedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _featureManager = featureManager;
            _logger = logger;
        }

        public async Task Handle(
            DecisionCreatedEvent domainEvent,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain event handled: {DomainEvent}", domainEvent.GetType().Name);

            //if (await _featureManager.IsEnabledAsync("DecisionIntegration"))
            //{

            var integrationEvent = domainEvent.ToDecisionCreatedIntegrationEvent();

            // الآن _publishEndpoint هو (ScopedEntityPublishEndpoint)
            // سيقوم بإضافة الرسالة إلى جدول OutboxMessage بدلاً من إرسالها للشبكة
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);

            _logger.LogInformation("✅ Message added to Outbox Context.");

            //}
        }
    }
}