using BuildingBlocks.Contracts.SecurityEvents;
using BuildingBlocks.Shared.Authorization;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Monitoring.Application.Consumers
{
    public class PermissionUpdateConsumer : IConsumer<UserPermissionsChangedIntegrationEvent>
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionUpdateConsumer> _logger;

        public PermissionUpdateConsumer(
            IPermissionService permissionService,
            ILogger<PermissionUpdateConsumer> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserPermissionsChangedIntegrationEvent> context)
        {
            var userId = context.Message.UserId;
            _logger.LogInformation($"Received Permission Update Event for User: {userId}. Invalidating Cache...");

            // استدعاء دالة المسح في خدمة الكاش
            await _permissionService.InvalidateCacheAsync(userId);
        }
    }

}
