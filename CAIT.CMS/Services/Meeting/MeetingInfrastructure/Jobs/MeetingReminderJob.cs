using BuildingBlocks.Contracts.Notifications;
using BuildingBlocks.Shared.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MeetingInfrastructure.Jobs
{
    public class MeetingReminderJob : IJob
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MeetingReminderJob> _logger;

        // 2. حقن الداتابيز في البناء
        public MeetingReminderJob(
            IPublishEndpoint publishEndpoint,
            IUnitOfWork unitOfWork,
            ILogger<MeetingReminderJob> logger)
        {
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var data = context.MergedJobDataMap;

                if (!Guid.TryParse(data.GetString("MeetingId"), out var meetingId))
                {
                    _logger.LogError("MeetingReminderJob: Invalid or missing MeetingId.");
                    return;
                }

                var title = data.GetString("Title") ?? "Unknown Meeting";
                var type = data.GetString("Type") ?? "General Reminder";

                _logger.LogInformation("⏰ Executing Reminder Job for Meeting {Id} - Type: {Type}", meetingId, type);

                // 3. نشر الحدث (هنا يتم إضافته للـ DbContext في الذاكرة فقط)
                await _publishEndpoint.Publish(new SendNotificationIntegrationEvent
                {
                    Type = "MeetingReminder",
                    ReferenceId = meetingId,
                    Message = $"🔔 Reminder: Your meeting '{title}' is starting in {type}.",
                    RecipientRole = "Attendee",
                    SentAt = DateTime.UtcNow
                }, context.CancellationToken);

                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                _logger.LogInformation("✅ Reminder sent to Outbox successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute MeetingReminderJob.");
                // إعادة المحاولة في Quartz تعتمد على التكوين، يمكن عمل throw هنا لو أردت إعادة المحاولة
            }
        }
    }
}