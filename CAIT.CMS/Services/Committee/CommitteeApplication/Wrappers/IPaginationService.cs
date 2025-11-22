namespace CommitteeApplication.Wrappers
{
    public interface IPaginationService
    {
        Task<PaginatedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize);

        Task<PaginatedResult<T>> PaginateListAsync<T>(List<T> source, int pageNumber, int pageSize);

    }
}

