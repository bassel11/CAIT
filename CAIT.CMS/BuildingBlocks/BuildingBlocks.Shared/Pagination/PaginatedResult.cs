namespace BuildingBlocks.Shared.Pagination
{
    public class PaginatedResult<TEntity> where TEntity : class
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public long TotalCount { get; }
        public IEnumerable<TEntity> Items { get; }

        public PaginatedResult(int pageIndex, int pageSize, long totalCount, IEnumerable<TEntity> items)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }
    }
}
