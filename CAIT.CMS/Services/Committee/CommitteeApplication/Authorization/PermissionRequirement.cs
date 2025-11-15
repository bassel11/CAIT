using Microsoft.AspNetCore.Authorization;

namespace CommitteeApplication.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }
        public PermissionRequirement(string permissionName)
        {
            PermissionName = permissionName;
        }
    }
}
