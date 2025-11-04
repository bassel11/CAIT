using Identity.Application.Common;
using Identity.Core.Entities;

namespace Identity.Application.DTOs.Users
{
    public class UserFilterDto : BaseFilterDto
    {
        public bool? IsActive { get; set; }
        public ApplicationUser.AuthenticationType? AuthType { get; set; }
        public bool? MFAEnabled { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }
}
