namespace Identity.Application.Common
{
    public abstract class BaseFilter
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = PaginationDefaults.DefaultPageSize;
        public string? SortBy { get; set; } = "createdAt";
        public string SortDir { get; set; } = "asc";
    }
}
}
