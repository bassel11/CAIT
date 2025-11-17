using CommitteeApplication.Filtering;
using System.Linq.Expressions;

namespace CommitteeApplication.Extensions
{
    public static class DynamicFilteringExtensions
    {
        public static IQueryable<T> ApplyDynamicFilters<T>(
            this IQueryable<T> query,
            Dictionary<string, string>? filters)
        {
            var criteriaList = FilterParser.Parse(filters);

            foreach (var criteria in criteriaList)
            {
                query = query.Where(BuildExpression<T>(criteria));
            }

            return query;
        }

        private static Expression<Func<T, bool>> BuildExpression<T>(FilterCriteria f)
        {
            var param = Expression.Parameter(typeof(T), "x");

            // الوصول إلى الخاصية حتى لو كانت Nested
            Expression property = param;
            foreach (var member in f.PropertyName.Split('.'))
            {
                property = Expression.PropertyOrField(property, member);
            }

            // تحويل القيمة حسب نوع الحقل
            var value = ConvertValue(property.Type, f.Value);
            Expression constant = Expression.Constant(value);

            // معالجة Nullable types
            if (Nullable.GetUnderlyingType(property.Type) != null && constant.Type != property.Type)
            {
                constant = Expression.Convert(constant, property.Type);
            }

            Expression body = f.Operator switch
            {
                FilterOperator.Equals =>
                    Expression.Equal(property, constant),

                FilterOperator.Contains =>
                    Expression.Call(property,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                        constant),

                FilterOperator.StartsWith =>
                    Expression.Call(property,
                        typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!,
                        constant),

                FilterOperator.EndsWith =>
                    Expression.Call(property,
                        typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!,
                        constant),

                FilterOperator.GreaterThan =>
                    Expression.GreaterThan(property, constant),

                FilterOperator.LessThan =>
                    Expression.LessThan(property, constant),

                _ => Expression.Equal(property, constant)
            };

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static object ConvertValue(Type type, string value)
        {
            var underlying = Nullable.GetUnderlyingType(type) ?? type;

            if (underlying == typeof(int))
                return int.Parse(value);

            if (underlying == typeof(decimal))
                return decimal.Parse(value);

            if (underlying == typeof(double))
                return double.Parse(value);

            if (underlying == typeof(float))
                return float.Parse(value);

            if (underlying == typeof(long))
                return long.Parse(value);

            if (underlying == typeof(DateTime))
                return DateTime.Parse(value);

            if (underlying == typeof(Guid))
                return Guid.Parse(value);

            return value;
        }


    }
}
