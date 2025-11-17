namespace CommitteeApplication.Common
{
    public interface IPaginationRequest
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }

        string? Search { get; set; }

        string? SortBy { get; set; }  // Example: "Name:asc,CreatedAt:desc"

        Dictionary<string, string>? Filters { get; set; }
    }
}
