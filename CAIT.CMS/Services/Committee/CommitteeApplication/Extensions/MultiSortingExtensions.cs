using System.Linq.Expressions;

namespace CommitteeApplication.Extensions
{
    public static class MultiSortingExtensions
    {
        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string? sortExpression)
        {
            if (string.IsNullOrWhiteSpace(sortExpression))
                return query;

            var sorts = sortExpression.Split(',');

            IOrderedQueryable<T>? orderedQuery = null;

            foreach (var item in sorts)
            {
                var parts = item.Split(':');
                string property = parts[0];
                bool desc = parts.Length > 1 && parts[1].ToLower() == "desc";

                orderedQuery = ApplySingleSort(query, orderedQuery, property, desc);
            }

            return orderedQuery ?? query;
        }

        private static IOrderedQueryable<T> ApplySingleSort<T>(
            IQueryable<T> query,
            IOrderedQueryable<T>? ordered,
            string propertyName,
            bool desc)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression property = param;

            foreach (var segment in propertyName.Split('.'))
            {
                property = Expression.PropertyOrField(property, segment);
            }

            var lambda = Expression.Lambda(property, param);

            string method = ordered == null
                ? (desc ? "OrderByDescending" : "OrderBy")
                : (desc ? "ThenByDescending" : "ThenBy");

            return (IOrderedQueryable<T>)typeof(Queryable)
                .GetMethods()
                .Single(m => m.Name == method && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type)
                .Invoke(null, new object[] { ordered ?? query, lambda })!;
        }
    }
}
