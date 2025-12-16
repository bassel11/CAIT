using MassTransit;
using MassTransit.DependencyInjection;
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
            // بدلاً من طلب IPublishEndpoint، نطلب Bind<TContext, IPublishEndpoint>
            Bind<ApplicationDbContext, IPublishEndpoint> publishEndpointBinder,
            IFeatureManager featureManager,
            ILogger<DecisionCreatedEventHandler> logger)
        {
            // نستخرج الناشر الصحيح من الـ Binder
            _publishEndpoint = publishEndpointBinder.Value;

            _featureManager = featureManager;
            _logger = logger;
        }

        public async Task Handle(
            DecisionCreatedEvent domainEvent,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain event handled: {DomainEvent}", domainEvent.GetType().Name);

            var integrationEvent = domainEvent.ToDecisionCreatedIntegrationEvent();

            // الآن _publishEndpoint هو (ScopedEntityPublishEndpoint)
            // سيقوم بإضافة الرسالة إلى جدول OutboxMessage بدلاً من إرسالها للشبكة
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);

            _logger.LogInformation("✅ Message added to Outbox Context.");
        }
    }
}