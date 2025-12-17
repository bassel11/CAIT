namespace BuildingBlocks.Shared.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        bool IsSuperAdmin { get; }
        IEnumerable<string> Roles { get; }
        bool IsInRole(string role);
    }
}
