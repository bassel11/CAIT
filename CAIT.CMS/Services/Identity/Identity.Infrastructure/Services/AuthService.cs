using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Identity.Core.Entities.ApplicationUser;

namespace Identity.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;
        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IConfiguration config)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<(bool Success, LoginResponseDto? Response, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AuthType = AuthenticationType.Database,
                MFAEnabled = false,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return (false, null, result.Errors.Select(e => e.Description));

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Member");

            var token = GenerateJwtToken(user, out var expiry);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = expiry
            };

            return (true, response, null);
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return (false, null, "Invalid credentials");

            if (!user.IsActive) return (false, null, "User is inactive");

            var token = GenerateJwtToken(user, out var expiry);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = expiry
            }, null);
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string token, string refreshToken)
        {
            var storedToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken && !r.Revoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return (false, null, "Invalid or expired refresh token");

            var user = storedToken.User;
            var newToken = GenerateJwtToken(user, out var expiry);
            var newRefreshToken = await GenerateRefreshTokenAsync(user);

            // Revoke old refresh token
            storedToken.Revoked = true;
            await _dbContext.SaveChangesAsync();

            return (true, new LoginResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiry = expiry
            }, null);
        }

        #region Private Methods

        private string GenerateJwtToken(ApplicationUser user, out DateTime expiry)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("uid", user.Id.ToString())
            };

            // Add roles
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenExpiryMinutes = Convert.ToDouble(_config["Jwt:DurationInMinutes"]);
            expiry = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
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

        #endregion
    }
}
