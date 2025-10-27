using System.Linq.Expressions;
namespace Identity.Application.Common
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, BaseFilter filter)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1
                ? PaginationDefaults.DefaultPageSize
                : Math.Min(filter.PageSize, PaginationDefaults.MaxPageSize);

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy, string sortDir,
            Dictionary<string, Expression<Func<T, object>>> sortMap)
        {
            if (string.IsNullOrWhiteSpace(sortBy) || !sortMap.ContainsKey(sortBy.ToLower()))
                sortBy = sortMap.Keys.First(); // fallback

            var keySelector = sortMap[sortBy.ToLower()];
            return sortDir.ToLower() == "desc"
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }
    }
}
