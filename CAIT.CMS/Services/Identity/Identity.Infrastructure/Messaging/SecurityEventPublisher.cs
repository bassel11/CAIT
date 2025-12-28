using BuildingBlocks.Contracts.SecurityEvents;
using Identity.Application.Security.SecurityEventPublisher;
using MassTransit;

namespace Identity.Infrastructure.Messaging
{
    public class SecurityEventPublisher : ISecurityEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public SecurityEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T securityEvent) where T : SecurityEventBase
        {
            await _publishEndpoint.Publish(securityEvent);
        }
    }

}
