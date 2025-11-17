namespace CommitteeApplication.Filtering
{
    public class FilterCriteria
    {
        public string PropertyName { get; set; }
        public FilterOperator Operator { get; set; }
        public string Value { get; set; }
    }
}
