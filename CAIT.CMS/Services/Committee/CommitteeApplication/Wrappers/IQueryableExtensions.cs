namespace CommitteeApplication.Wrappers
{
    public interface IQueryableExtensions
    {
        Task<PaginatedResult<T>> PaginateAsync<T>(
                    IQueryable<T> query,
                    int pageNumber,
                    int pageSize
                ) where T : class;
    }
}
