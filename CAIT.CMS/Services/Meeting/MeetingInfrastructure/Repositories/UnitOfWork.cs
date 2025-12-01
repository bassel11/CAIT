using MeetingApplication.Interfaces;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MeetingInfrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeetingDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        public UnitOfWork(MeetingDbContext dbContext) { _dbContext = dbContext; }


        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);
        }


        public async Task CommitAsync(CancellationToken ct = default)
        {
            try
            {
                await _dbContext.SaveChangesAsync(ct);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(ct);
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // إذا حصل تضارب في التزامن
                await RollbackAsync(ct);
                throw new InvalidOperationException("تم تعديل محضر الاجتماع من قبل مستخدم آخر، الرجاء إعادة المحاولة.", ex);
            }

            catch
            {
                await RollbackAsync(ct);
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(ct);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
