using MeetingCore.Repositories;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeetingInfrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : class
    {
        protected readonly MeetingDbContext _dbContext;

        public RepositoryBase(MeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> GetTableNoTracking()
        {
            return _dbContext.Set<T>().AsNoTracking();
        }

        public async Task<List<T>> GetAllNoTrackingAsync()
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public IQueryable<T> GetTable()
        {
            return _dbContext.Set<T>();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public IQueryable<T> Query()
        {
            return _dbContext.Set<T>().AsNoTracking();
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return _dbContext.SaveChangesAsync(ct);
        }
    }
}
