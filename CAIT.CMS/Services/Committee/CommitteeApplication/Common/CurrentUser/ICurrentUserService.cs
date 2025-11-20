namespace CommitteeApplication.Common.CurrentUser
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        IEnumerable<string> Roles { get; }
        //IEnumerable<string> Permissions { get; } // لو كنت تضع claim "permissions"
        bool IsAuthenticated { get; }
        bool IsSuperAdmin { get; }
        bool IsInRole(string role);
        //bool HasPermission(string permission);
    }
}
