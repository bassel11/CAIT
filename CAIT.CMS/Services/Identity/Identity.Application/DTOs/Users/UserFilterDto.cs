using Identity.Core.Entities;

namespace Identity.Application.DTOs.Users
{
    public class UserFilterDto
    {
        public string? Search { get; set; }          // Name or email
        public bool? IsActive { get; set; }
        public ApplicationUser.AuthenticationType? AuthType { get; set; }
        public bool? MFAEnabled { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
