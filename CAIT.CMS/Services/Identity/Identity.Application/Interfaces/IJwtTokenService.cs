using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(ApplicationUser user, out DateTime expiry);
    }
}
