using MediatR;
using TaskApplication.Common.Interfaces;
using TaskCore.Events.AutomationEvents;

namespace TaskApplication.Features.Automation.Commands.SendReminders
{
    public class SendRemindersCommandHandler : IRequestHandler<SendRemindersCommand>
    {
        private readonly ITaskRepository _repository;
        private readonly IPublisher _publisher;

        public SendRemindersCommandHandler(ITaskRepository repository, IPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task Handle(SendRemindersCommand request, CancellationToken cancellationToken)
        {
            // الأيام المطلوبة للتذكير: 7، 3، 1
            int[] reminderDays = { 1, 3, 7 };

            foreach (var days in reminderDays)
            {
                var tasks = await _repository.GetReminderCandidatesAsync(days, cancellationToken);

                foreach (var task in tasks)
                {
                    // إنشاء حدث التذكير
                    // ملاحظة: هنا لا نعدل حالة المهمة، فقط نرسل إشعاراً
                    var reminderEvent = new TaskDeadlineApproachingEvent(
                        task.Id.Value,
                        task.Title.Value,
                        task.Deadline!.Value,
                        days,
                        task.TaskAssignees.Select(a => a.UserId.Value).ToList()
                    );

                    // نشر الحدث فوراً (لأنه لا يوجد SaveChanges هنا)
                    await _publisher.Publish(reminderEvent, cancellationToken);

                    await _repository.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
