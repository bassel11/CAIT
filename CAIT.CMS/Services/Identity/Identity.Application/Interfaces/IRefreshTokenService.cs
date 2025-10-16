using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
    }
}
