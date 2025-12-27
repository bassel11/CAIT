using BuildingBlocks.Contracts.Task.IntegrationEvents;
using MassTransit;
using Monitoring.Application.Data;
using Monitoring.Core.Entities;

namespace Monitoring.Application.Consumers
{
    // عند إسناد مهمة لعضو (لتحديث عبء العمل)
    public class TaskAssignedConsumer : IConsumer<TaskAssignedIntegrationEvent>
    {
        private readonly IMonitoringDbContext _context;
        public TaskAssignedConsumer(IMonitoringDbContext context) => _context = context;

        public async Task Consume(ConsumeContext<TaskAssignedIntegrationEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;

            // 1. Member workload
            var workload = await _context.MemberWorkloads
                .FindAsync(new object[] { message.MemberId }, cancellationToken);

            if (workload == null)
            {
                workload = new MemberWorkload
                {
                    MemberId = message.MemberId,
                    MemberName = message.MemberName,
                    PendingTasks = 0
                };

                _context.MemberWorkloads.Add(workload);
            }

            workload.PendingTasks++;

            // 2. Committee activity
            var committee = await _context.CommitteeSummaries
                .FindAsync(new object[] { message.CommitteeId }, cancellationToken);

            if (committee != null)
            {
                committee.LastActivityDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
