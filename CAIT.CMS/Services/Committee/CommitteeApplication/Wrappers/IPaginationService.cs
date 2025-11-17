namespace CommitteeApplication.Wrappers
{
    public interface IPaginationService
    {
        Task<PaginatedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize);
    }
}

