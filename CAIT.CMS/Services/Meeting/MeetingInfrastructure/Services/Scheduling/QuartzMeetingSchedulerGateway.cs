using MeetingApplication.Interfaces.Scheduling;
using MeetingInfrastructure.Jobs;
using Microsoft.Extensions.Logging; // مهم للوجينج
using Quartz;

namespace MeetingInfrastructure.Services.Scheduling
{
    public class QuartzMeetingSchedulerGateway : IMeetingSchedulerGateway
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzMeetingSchedulerGateway> _logger;

        public QuartzMeetingSchedulerGateway(
            ISchedulerFactory schedulerFactory,
            ILogger<QuartzMeetingSchedulerGateway> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task ScheduleMeetingRemindersAsync(Guid meetingId, string title, DateTime startDate, CancellationToken ct)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);
            var idStr = meetingId.ToString();
            var groupName = "MeetingReminders";

            // 1. تذكير قبل 24 ساعة
            // الشرط: أن يكون موعد التذكير في المستقبل
            var reminder24hTime = startDate.AddHours(-24);
            if (reminder24hTime > DateTime.UtcNow)
            {
                var jobKey = new JobKey($"Reminder_24H_{idStr}", groupName);

                // Idempotency: التحقق من الوجود أولاً
                if (!await scheduler.CheckExists(jobKey, ct))
                {
                    var job = JobBuilder.Create<MeetingReminderJob>()
                        .WithIdentity(jobKey)
                        .UsingJobData("MeetingId", idStr)
                        .UsingJobData("Title", title)
                        .UsingJobData("Type", "24 Hours")
                        .StoreDurably()
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"Trigger_24H_{idStr}", groupName)
                        .StartAt(reminder24hTime)
                        .Build();

                    await scheduler.ScheduleJob(job, trigger, ct);
                    _logger.LogInformation("Scheduled 24H reminder for meeting {Id}", meetingId);
                }
            }

            // 2. تذكير قبل 15 دقيقة (الجزء المكمل)
            var reminder15mTime = startDate.AddMinutes(-15);
            if (reminder15mTime > DateTime.UtcNow)
            {
                var jobKey = new JobKey($"Reminder_15M_{idStr}", groupName);

                if (!await scheduler.CheckExists(jobKey, ct))
                {
                    var job = JobBuilder.Create<MeetingReminderJob>()
                        .WithIdentity(jobKey)
                        .UsingJobData("MeetingId", idStr)
                        .UsingJobData("Title", title)
                        .UsingJobData("Type", "15 Minutes")
                        .StoreDurably()
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"Trigger_15M_{idStr}", groupName)
                        .StartAt(reminder15mTime)
                        .Build();

                    await scheduler.ScheduleJob(job, trigger, ct);
                    _logger.LogInformation("Scheduled 15M reminder for meeting {Id}", meetingId);
                }
            }
        }

        public async Task CancelMeetingRemindersAsync(Guid meetingId, CancellationToken ct)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);
            var idStr = meetingId.ToString();
            var groupName = "MeetingReminders";

            var keys = new List<JobKey>
            {
                new JobKey($"Reminder_24H_{idStr}", groupName),
                new JobKey($"Reminder_15M_{idStr}", groupName)
            };

            // حذف الوظائف إن وجدت
            await scheduler.DeleteJobs(keys, ct);
            _logger.LogInformation("Cancelled reminders for meeting {Id}", meetingId);
        }
    }
}