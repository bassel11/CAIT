//using MassTransit;
//using Monitoring.Application.Data;
//using Monitoring.Core.Entities;

//namespace Monitoring.Application.Consumers
//{
//    public class CommitteeCreatedConsumer : IConsumer<CommitteeCreatedEvent>
//    {
//        private readonly IMonitoringDbContext _context;
//        public CommitteeCreatedConsumer(IMonitoringDbContext context) => _context = context;

//        public async Task Consume(ConsumeContext<CommitteeCreatedEvent> context, CancellationToken cancellationToken)
//        {
//            var committee = new CommitteeSummary
//            {
//                Id = context.Message.CommitteeId,
//                Name = context.Message.Name,
//                Status = "Active",
//                CreatedAt = context.Message.CreatedAt,
//                LastActivityDate = DateTime.UtcNow,
//                IsCompliant = true
//            };
//            _context.CommitteeSummaries.Add(committee);
//            await _context.SaveChangesAsync(cancellationToken);
//        }
//    }

//    // عند إسناد مهمة لعضو (لتحديث عبء العمل)
//    public class TaskAssignedConsumer : IConsumer<TaskAssignedEvent>
//    {
//        private readonly IMonitoringDbContext _context;
//        public TaskAssignedConsumer(IMonitoringDbContext context) => _context = context;

//        public async Task Consume(ConsumeContext<TaskAssignedEvent> context, CancellationToken cancellationToken)
//        {
//            var workload = await _context.MemberWorkloads.FindAsync(context.Message.MemberId);
//            if (workload == null)
//            {
//                workload = new MemberWorkload
//                {
//                    MemberId = context.Message.MemberId,
//                    MemberName = context.Message.MemberName
//                };
//                _context.MemberWorkloads.Add(workload);
//            }

//            workload.PendingTasks++;

//            // تحديث نشاط اللجنة أيضاً
//            var committee = await _context.CommitteeSummaries.FindAsync(context.Message.CommitteeId);
//            if (committee != null) committee.LastActivityDate = DateTime.UtcNow;

//            await _context.SaveChangesAsync(cancellationToken);
//        }
//    }
//}
