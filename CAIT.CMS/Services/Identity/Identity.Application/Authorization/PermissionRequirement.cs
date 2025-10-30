using Microsoft.AspNetCore.Authorization;

namespace Identity.Application.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }
        public Guid? CommitteeId { get; }

        public PermissionRequirement(string permissionName, Guid? committeeId = null)
        {
            PermissionName = permissionName;
            CommitteeId = committeeId;
        }
    }
}
