using System.Text.RegularExpressions;

namespace MeetingApplication.Filtering
{
    public static class FilterParser
    {
        private static readonly Regex FilterRegex =
            new Regex(@"(?<prop>[\w\.]+):(?<op>eq|contains|startswith|endswith|gt|lt)",
                RegexOptions.IgnoreCase);

        public static List<FilterCriteria> Parse(Dictionary<string, string>? filters)
        {
            var list = new List<FilterCriteria>();

            if (filters == null)
                return list;

            foreach (var f in filters)
            {
                var match = FilterRegex.Match(f.Key);
                if (!match.Success) continue;

                var property = match.Groups["prop"].Value;
                var op = match.Groups["op"].Value.ToLower();

                var parsedOperator = op switch
                {
                    "eq" => FilterOperator.Equals,
                    "contains" => FilterOperator.Contains,
                    "startswith" => FilterOperator.StartsWith,
                    "endswith" => FilterOperator.EndsWith,
                    "gt" => FilterOperator.GreaterThan,
                    "lt" => FilterOperator.LessThan,
                    _ => FilterOperator.Equals
                };

                list.Add(new FilterCriteria
                {
                    PropertyName = property,
                    Operator = parsedOperator,
                    Value = f.Value
                });
            }

            return list;
        }
    }

}
