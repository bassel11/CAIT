using MediatR;
using Quartz;
using TaskApplication.Features.Automation.Commands.ProcessOverdueTasks;
using TaskApplication.Features.Automation.Commands.SendReminders;

namespace TaskInfrastructure.Jobs
{
    public class TaskEscalationJob : IJob
    {
        private readonly IMediator _mediator;
        public TaskEscalationJob(IMediator mediator) => _mediator = mediator;

        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new ProcessOverdueTasksCommand());
        }
    }

    // وظيفة التذكيرات (مرة واحدة يومياً - صباحاً)
    [DisallowConcurrentExecution]
    public class TaskReminderJob : IJob
    {
        private readonly IMediator _mediator;
        public TaskReminderJob(IMediator mediator) => _mediator = mediator;

        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new SendRemindersCommand());
        }
    }
}
