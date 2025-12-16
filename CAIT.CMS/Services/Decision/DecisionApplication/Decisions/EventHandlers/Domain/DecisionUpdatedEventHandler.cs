using DecisionApplication.Extensions;
using DecisionCore.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace DecisionApplication.Decisions.EventHandlers.Domain
{
    public class DecisionUpdatedEventHandler
        : INotificationHandler<DecisionUpdatedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<DecisionUpdatedEventHandler> _logger;

        public DecisionUpdatedEventHandler(
            IPublishEndpoint publishEndpoint,
            IFeatureManager featureManager,
            ILogger<DecisionUpdatedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _featureManager = featureManager;
            _logger = logger;
        }

        public async Task Handle(
            DecisionUpdatedEvent domainEvent,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Domain event handled: {DomainEvent}",
                domainEvent.GetType().Name);

            if (await _featureManager.IsEnabledAsync("DecisionIntegration"))
            {
                var integrationEvent =
                    domainEvent.ToDecisionUpdatedIntegrationEvent();

                await _publishEndpoint.Publish(
                    integrationEvent,
                    cancellationToken);
            }
        }
    }
}
