using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{

    // Record لاحتواء التوكن وتاريخ الانتهاء
    public record JwtTokenResult(string Token, DateTime Expiry);

    public interface IJwtTokenService
    {
        Task<JwtTokenResult> GenerateJwtTokenAsync(ApplicationUser user);
    }

    //public interface IJwtTokenService
    //{
    //    string GenerateJwtToken(ApplicationUser user, out DateTime expiry);
    //}
}
