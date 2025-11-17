using CommitteeApplication.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Services
{
    public class PaginationService : IPaginationService
    {
        public async Task<PaginatedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            int count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
    }
}
