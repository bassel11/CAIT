using BuildingBlocks.Contracts.Task.IntegrationEvents;
using MassTransit;
using NotificationService.Services;

namespace NotificationService.Consumers.SendEmail
{
    public class TaskEscalatedConsumer : IConsumer<TaskEscalatedIntegrationEvent>
    {
        private readonly IEmailService _emailService;
        //private readonly IUserService _userService; // لجلب إيميلات المستخدمين

        public TaskEscalatedConsumer(IEmailService emailService
            //,IUserService userService
            )
        {
            _emailService = emailService;
            // _userService = userService;
        }

        public async Task Consume(ConsumeContext<TaskEscalatedIntegrationEvent> context)
        {
            var message = context.Message;

            // 1. جلب بيانات التواصل (إيميل رئيس اللجنة)
            // خدمة الإشعارات قد تملك قاعدة بيانات للمستخدمين أو تتصل بـ Identity Service
            //var chairmanEmail = await _userService.GetCommitteeChairmanEmailAsync(message.CommitteeId);
            var chairmanEmail = "bassel.as19@gmail.com";

            if (string.IsNullOrEmpty(chairmanEmail)) return;

            // 2. تجهيز القالب (HTML Template)
            // هنا المكان الصحيح للـ HTML وليس في TaskService
            var emailBody = $@"
                <div style='border: 1px solid red; padding: 20px;'>
                    <h2 style='color: red;'>⚠️ Escalation Alert</h2>
                    <p>The task <strong>{message.TaskTitle}</strong> is overdue by {message.DaysOverdue} days.</p>
                    <p><strong>Deadline:</strong> {message.OriginalDeadline:yyyy-MM-dd}</p>
                    <a href='https://portal.mycompany.com/tasks/{message.TaskId}'>View Task</a>
                </div>";

            // 3. الإرسال الفعلي
            await _emailService.SendEmailAsync(
                to: chairmanEmail,
                subject: $"[Escalation] Overdue Task: {message.TaskTitle}",
                body: emailBody
            );

            // يمكن أيضاً إرسال Push Notification هنا
        }
    }
}
