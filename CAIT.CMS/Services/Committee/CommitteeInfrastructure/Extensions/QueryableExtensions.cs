using CommitteeApplication.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Extensions
{
    public class QueryableExtensions : IQueryableExtensions
    {
        public async Task<PaginatedResult<T>> PaginateAsync<T>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize
        ) where T : class
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            int count = await query.AsNoTracking().CountAsync();

            if (count == 0)
                return PaginatedResult<T>.Success(new List<T>(), count, pageNumber, pageSize);

            var items = await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
    }
}
