using BuildingBlocks.Contracts.Meeting.MoMs.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace TaskApplication.Consumers
{
    public class MoMApprovedConsumer : IConsumer<MoMApprovedIntegrationEvent>
    {
        private readonly ITaskRepository _repository; // أو IApplicationDbContext
        private readonly ILogger<MoMApprovedConsumer> _logger;

        public MoMApprovedConsumer(
            ITaskRepository repository,
            ILogger<MoMApprovedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MoMApprovedIntegrationEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("📥 [Task Service] Received MoM Approved Event: {MeetingId}", message.MeetingId);

            var momId = MoMId.Of(message.MoMId);
            var meetingId = MeetingId.Of(message.MeetingId);
            var committeeId = CommitteeId.Of(Guid.Empty);
            var exists = await _repository.TasksExistForMoMAsync(momId);

            if (exists)
            {
                _logger.LogWarning("Tasks for MoM {MoMId} already exist. Skipping.", message.MoMId);
                return;
            }

            // 3. التحقق من وجود مهام
            if (!message.Tasks.Any())
            {
                _logger.LogInformation("No tasks found in this MoM.");
                return;
            }

            // 4. إنشاء المهام
            foreach (var taskDto in message.Tasks)
            {
                var task = TaskItem.CreateFromMoM(
                    id: TaskItemId.Of(Guid.NewGuid()),
                    title: TaskTitle.Of(taskDto.Title),
                    description: TaskDescription.Of(taskDto.Title), // استخدام العنوان كوصف مبدئي
                    deadline: TaskDeadline.Of(taskDto.DueDate),
                    committeeId: committeeId, // ⚠️ هام
                    meetingId: meetingId,
                    momId: momId
                );

                // إسناد الموظف (Assignee)
                if (taskDto.AssigneeId != Guid.Empty)
                {
                    task.AssignUser(UserId.Of(taskDto.AssigneeId), "system-import@domain.com", "System Import");
                }

                await _repository.AddAsync(task, context.CancellationToken);
            }

            await _repository.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("[Task Service] Created {Count} tasks from MoM.", message.Tasks.Count);
        }
    }
}
