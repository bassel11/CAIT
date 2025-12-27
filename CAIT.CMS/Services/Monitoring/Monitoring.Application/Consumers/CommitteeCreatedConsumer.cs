namespace Monitoring.Application.Consumers
{
    //public class CommitteeCreatedConsumer : IConsumer<CommitteeCreatedEvent>
    //{
    //    private readonly IMonitoringDbContext _context;
    //    public CommitteeCreatedConsumer(IMonitoringDbContext context)
    //        => _context = context;

    //    public async Task Consume(ConsumeContext<CommitteeCreatedEvent> context
    //                            , CancellationToken cancellationToken)
    //    {
    //        var committee = new CommitteeSummary
    //        {
    //            Id = context.Message.CommitteeId,
    //            Name = context.Message.Name,
    //            Status = "Active",
    //            CreatedAt = context.Message.CreatedAt,
    //            LastActivityDate = DateTime.UtcNow,
    //            IsCompliant = true
    //        };
    //        _context.CommitteeSummaries.Add(committee);
    //        await _context.SaveChangesAsync(cancellationToken);
    //    }
    //}

}
