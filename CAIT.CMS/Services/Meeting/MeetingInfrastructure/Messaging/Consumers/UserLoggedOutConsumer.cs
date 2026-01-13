using BuildingBlocks.Contracts.SecurityEvents;
using BuildingBlocks.Shared.Authorization;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MeetingInfrastructure.Messaging.Consumers
{
    public class UserLoggedOutConsumer : IConsumer<UserLoggedOutEvent>
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<UserLoggedOutConsumer> _logger;

        public UserLoggedOutConsumer(
            IPermissionService permissionService,
            ILogger<UserLoggedOutConsumer> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserLoggedOutEvent> context)
        {
            var userId = context.Message.UserId;

            _logger.LogInformation($"⚡ (Meeting Service) Received Logout Event for User: {userId}. Invalidating Cache...");

            // حذف الكاش فوراً
            await _permissionService.InvalidateCacheAsync(userId);
        }
    }

}
