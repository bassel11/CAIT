using BuildingBlocks.Contracts.Task.IntegrationEvents;
using MassTransit;
using NotificationService.Services;

namespace NotificationService.Consumers.SendEmail
{
    public class TaskReminderConsumer : IConsumer<TaskReminderIntegrationEvent>
    {
        private readonly IEmailService _emailService;
        // private readonly IUserService _userService; 

        public TaskReminderConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<TaskReminderIntegrationEvent> context)
        {
            var message = context.Message;

            // 1. تحديد المستلمين (الموظفين المسؤولين عن المهمة)
            // في الواقع: نقوم بجلب قائمة الإيميلات بناءً على message.AssigneeIds
            // var emails = await _userService.GetUsersEmailsAsync(message.AssigneeIds);

            // للتجربة الآن: سنرسل لنفس الإيميل الخاص بك
            var targetEmails = new List<string> { "bassel.as19@gmail.com" };

            if (!targetEmails.Any()) return;

            // 2. تحديد لون التنبيه بناءً على الأيام المتبقية
            // أحمر إذا بقي يوم، برتقالي إذا 3، أزرق إذا 7
            string color = message.DaysRemaining <= 1 ? "red" : (message.DaysRemaining <= 3 ? "orange" : "blue");
            string urgencyText = message.DaysRemaining <= 1 ? "URGENT" : "Reminder";

            // 3. تجهيز القالب (HTML Template)
            var emailBody = $@"
                <div style='border: 1px solid {color}; padding: 20px; font-family: Arial, sans-serif;'>
                    <h2 style='color: {color};'>⏰ Task Deadline {urgencyText}</h2>
                    <p>Hello,</p>
                    <p>This is a reminder that the deadline for the following task is approaching:</p>
                    <ul>
                        <li><strong>Task:</strong> {message.TaskTitle}</li>
                        <li><strong>Deadline:</strong> {message.Deadline:yyyy-MM-dd}</li>
                        <li><strong>Time Remaining:</strong> <span style='color:{color}; font-weight:bold;'>{message.DaysRemaining} Day(s)</span></li>
                    </ul>
                    <br/>
                    <a href='https://portal.mycompany.com/tasks/{message.TaskId}' style='background-color:{color}; color:white; padding:10px 15px; text-decoration:none; border-radius:5px;'>View Task</a>
                </div>";

            // 4. الإرسال لكل المعينين
            foreach (var email in targetEmails)
            {
                await _emailService.SendEmailAsync(
                    to: email,
                    subject: $"[{urgencyText}] Upcoming Deadline: {message.TaskTitle}",
                    body: emailBody
                );
            }
        }
    }
}
