using DecisionApplication.Extensions;
using DecisionCore.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace DecisionApplication.Decisions.EventHandlers.Domain
{
    public class DecisionDeletedEventHandler
        : INotificationHandler<DecisionDeletedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<DecisionDeletedEventHandler> _logger;

        public DecisionDeletedEventHandler(
            IPublishEndpoint publishEndpoint,
            IFeatureManager featureManager,
            ILogger<DecisionDeletedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _featureManager = featureManager;
            _logger = logger;
        }

        public async Task Handle(
            DecisionDeletedEvent domainEvent,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Domain event handled: {DomainEvent}",
                domainEvent.GetType().Name);

            if (await _featureManager.IsEnabledAsync("DecisionIntegration"))
            {
                var integrationEvent =
                    domainEvent.ToDecisionDeletedIntegrationEvent();

                await _publishEndpoint.Publish(
                    integrationEvent,
                    cancellationToken);
            }
        }
    }
}
