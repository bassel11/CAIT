using BuildingBlocks.Contracts.SecurityEvents;

namespace Identity.Application.Security.SecurityEventPublisher
{
    public interface ISecurityEventPublisher
    {
        Task PublishAsync<T>(T securityEvent) where T : SecurityEventBase;
    }

}
