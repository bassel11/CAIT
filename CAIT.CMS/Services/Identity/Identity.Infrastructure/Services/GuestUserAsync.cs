using Identity.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Infrastructure.Services
{
    public class GuestUserAsync : IGuestUserAsync
    {
        public Task<bool> IsGuestUserAsync(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            // إذا كان idp موجود ⇒ Guest
            if (jwt.Claims.Any(c => c.Type == "idp"))
                return Task.FromResult(true);

            // أو فحص userType من Graph API
            // return await GetUserTypeFromGraphAsync(token) == "Guest";

            return Task.FromResult(false);
        }
    }
}
