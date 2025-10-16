using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;

namespace Identity.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            return refreshToken.Token;
        }
    }
}
