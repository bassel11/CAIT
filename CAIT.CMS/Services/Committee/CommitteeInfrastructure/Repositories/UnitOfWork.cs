using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CommitteeInfrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommitteeContext _context;
        private IDbContextTransaction? _currentTransaction;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(CommitteeContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return;

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("----- Begin transaction {TransactionId}", _currentTransaction.TransactionId);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // ملاحظة: الـ Repositories تقوم بـ SaveChanges، لذا هنا نعتمد الترانزكشن فقط
                await _currentTransaction?.CommitAsync(cancellationToken)!;

                _logger.LogInformation("----- Commit transaction {TransactionId}", _currentTransaction?.TransactionId);
            }
            catch
            {
                await RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(cancellationToken)!;
                _logger.LogWarning("----- Rollback transaction {TransactionId}", _currentTransaction?.TransactionId);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
