using BuildingBlocks.Shared.Abstractions;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeetingDbContext _dbContext;

        public UnitOfWork(MeetingDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // بفضل MassTransit Outbox Configuration في DbContext
            // هذا السطر يضمن حفظ البيانات ورسائل الأحداث في Transaction واحد تلقائياً
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
