namespace BuildingBlocks.Shared.Authorization
{
    public class PermissionSnapshot
    {
        public Guid UserId { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public bool Has(string permission, Guid? resourceId = null)
        {
            // لو أردت دعم ResourceId
            return Permissions.Contains(permission);
        }
    }

}
