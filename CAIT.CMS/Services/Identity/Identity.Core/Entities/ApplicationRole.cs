using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();


        #region Pre for new Privilage method
        public virtual ICollection<UserRolePermReso> UserRolePermResos { get; set; } = new List<UserRolePermReso>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();


        #endregion
    }
}
