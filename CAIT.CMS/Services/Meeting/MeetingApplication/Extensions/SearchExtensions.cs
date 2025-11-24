using System.Linq.Expressions;

namespace MeetingApplication.Extensions
{
    public static class SearchExtensions
    {
        public static IQueryable<T> ApplySearch<T>(
            this IQueryable<T> query,
            string? search,
            Expression<Func<T, bool>> predicate)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            return query.Where(predicate);
        }
    }

}
