namespace BuildingBlocks.Shared.Authorization
{
    public interface IHttpPermissionFetcher
    {
        Task<PermissionSnapshot> FetchAsync(Guid userId, string serviceName);
    }

}
