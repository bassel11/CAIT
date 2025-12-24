using MediatR;
using TaskApplication.Common.Interfaces;
using TaskCore.Events.AutomationEvents;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Automation.Commands.ProcessOverdueTasks
{
    public class ProcessOverdueTasksCommandHandler : IRequestHandler<ProcessOverdueTasksCommand>
    {
        private readonly ITaskRepository _repository;
        private readonly IPublisher _publisher; // لنشر الأحداث

        public ProcessOverdueTasksCommandHandler(ITaskRepository repository, IPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task Handle(ProcessOverdueTasksCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب المرشحين
            var overdueTasks = await _repository.GetOverdueCandidatesAsync(cancellationToken);

            foreach (var task in overdueTasks)
            {
                // 2. تحديث الحالة في الدومين (Logic)
                // System User GUID (معرف النظام الثابت)
                var systemUserId = UserId.Of(Guid.Parse("00000000-0000-0000-0000-000000000001"));

                // هذه الدالة (التي كتبناها سابقاً في TaskItem) ستقوم بتغيير الحالة إلى Overdue
                task.CheckOverdue(systemUserId);

                // 3. إعداد بيانات التصعيد للإشعار
                var daysOverdue = (DateTime.UtcNow - task.Deadline!.Value).Days;

                var escalationEvent = new TaskEscalatedEvent(
                    task.Id.Value,
                    task.CommitteeId.Value,
                    task.Title.Value,
                    task.Deadline.Value,
                    daysOverdue == 0 ? 1 : daysOverdue, // على الأقل يوم واحد
                    task.TaskAssignees.Select(a => a.UserId.Value).ToList()
                );

                // 4. نشر الحدث (ليتم التقاطه وإرسال الإيميل للرئيس)
                task.AddDomainEvent(escalationEvent);
            }

            // 5. حفظ الكل دفعة واحدة
            if (overdueTasks.Any())
            {
                await _repository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
