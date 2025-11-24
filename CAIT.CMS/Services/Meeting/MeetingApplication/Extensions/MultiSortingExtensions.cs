using System.Linq.Expressions;

namespace MeetingApplication.Extensions
{
    public static class MultiSortingExtensions
    {
        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string? sortExpression,
            string defaultSort)
        {
            if (string.IsNullOrWhiteSpace(sortExpression))
                return query;

            var sortParts = sortExpression.Split(',');

            IOrderedQueryable<T>? orderedQuery = null;

            foreach (var part in sortParts)
            {
                var pieces = part.Split(':', StringSplitOptions.RemoveEmptyEntries);
                string propertyPath = pieces[0].Trim();
                bool desc = pieces.Length > 1 && pieces[1].Trim().ToLower() == "desc";

                orderedQuery = ApplySingleSort(query, orderedQuery, propertyPath, desc);
            }

            return orderedQuery ?? query;
        }

        private static IOrderedQueryable<T> ApplySingleSort<T>(
            IQueryable<T> baseQuery,
            IOrderedQueryable<T>? orderedQuery,
            string propertyPath,
            bool desc)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression body = param;

            foreach (var segment in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, segment);
            }

            var lambda = Expression.Lambda(body, param);

            string method = orderedQuery == null
                ? (desc ? "OrderByDescending" : "OrderBy")
                : (desc ? "ThenByDescending" : "ThenBy");

            return (IOrderedQueryable<T>)typeof(Queryable)
                .GetMethods()
                .Single(m => m.Name == method &&
                            m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), body.Type)
                .Invoke(null, new object[] { orderedQuery ?? baseQuery, lambda })!;
        }
    }

}
