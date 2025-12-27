using BuildingBlocks.Contracts.Task.IntegrationEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskCore.Events.TaskAssigneeEvents;

namespace TaskApplication.Features.Tasks.EventHandlers.Domain
{
    public class TaskAssignUserEventHandler
        : INotificationHandler<TaskAssignUserEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<TaskAssignUserEventHandler> _logger;

        public TaskAssignUserEventHandler(
            IPublishEndpoint publishEndpoint,
            ILogger<TaskAssignUserEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(
            TaskAssignUserEvent notification,
            CancellationToken cancellationToken)
        {
            var integrationEvent = new TaskAssignedIntegrationEvent
            {
                TaskId = notification.TaskId.Value,
                //CommitteeId = notification.Assignee.CommitteeId.Value,

                MemberId = notification.Assignee.UserId.Value,
                MemberName = notification.Assignee.Name,
                MemberEmail = notification.Assignee.Email,

                AssignedAtUtc = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(
                integrationEvent,
                cancellationToken);

            _logger.LogInformation(
                "TaskAssignedIntegrationEvent published for TaskId {TaskId}, MemberId {MemberId}",
                integrationEvent.TaskId,
                integrationEvent.MemberId);
        }
    }
}
