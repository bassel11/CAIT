using System.Linq.Expressions;

namespace MeetingCore.Repositories
{
    public interface IAsyncRepository<T> where T : class
    {
        // Queryable
        IQueryable<T> GetTable();
        IQueryable<T> GetTableNoTracking();

        Task<List<T>> GetAllNoTrackingAsync();
        // Get All
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        // Get by Id
        Task<T> GetByIdAsync(Guid id);

        // Add / Update / Delete
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
        IQueryable<T> Query();
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
