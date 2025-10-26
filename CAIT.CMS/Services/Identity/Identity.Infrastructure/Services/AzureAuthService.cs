using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Infrastructure.Services
{
    public class AzureAuthService : IAzureAuthService
    {
        private readonly ILogger<AzureAuthService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public AzureAuthService(
            ILogger<AzureAuthService> logger,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService,
            IConfiguration config,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _config = config;
            _db = db;
            _logger = logger;
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> ExchangeAzureTokenAsync(string azureAccessToken)
        {
            var azureSection = _config.GetSection("AzureAd");
            var authority = $"{azureSection["Instance"]}{azureSection["TenantId"]}/v2.0";
            var audience = azureSection["Audience"];
            var validIssuers = azureSection.GetSection("ValidIssuers").Get<string[]>();

            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{authority}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());

            var openIdConfig = await configManager.GetConfigurationAsync(CancellationToken.None);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = audience,
                ValidIssuers = validIssuers,
                IssuerSigningKeys = openIdConfig.SigningKeys,
                NameClaimType = "preferred_username",
                RoleClaimType = "roles"
            };

            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            SecurityToken validatedToken;

            try
            {
                principal = handler.ValidateToken(azureAccessToken, validationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return (false, null, $"Invalid Azure Token: {ex.Message}", null);
            }

            // Extract claims safely
            //var oid = principal.FindFirst("oid")?.Value ?? principal.FindFirst("sub")?.Value;
            //var email = principal.FindFirst("preferred_username")?.Value ?? principal.FindFirst("upn")?.Value;
            //var tid = principal.FindFirst("tid")?.Value ?? principal.FindFirst("tenantId")?.Value;

            //if (string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(tid))
            //    return (false, null, "Azure token missing oid/tid claims");


            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null)
                return (false, null, "Invalid JWT format", null);

            var oid = jwtToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;
            var tid = jwtToken.Claims.FirstOrDefault(c => c.Type == "tid")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value
                          ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "upn")?.Value
                          ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                          ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            var firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value
             ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "first_name")?.Value
             ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value
             ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "display_name")?.Value;

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                firstName = firstName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }


            if (!Guid.TryParse(oid, out var objectId) || !Guid.TryParse(tid, out var tenantId))
                return (false, null, "Invalid GUID format in token claims", null);
            // Find or create user
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.AzureObjectId == Guid.Parse(oid));

            // التحقق من صلاحية الحساب
            if (user.ExpirationDate.HasValue && user.ExpirationDate < DateTime.UtcNow)
                return (false, null, "Account expired", null);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    FirstName = firstName,
                    Email = email,
                    AuthType = ApplicationUser.AuthenticationType.AzureAD,
                    AzureObjectId = objectId,
                    AzureTenantId = tenantId,
                    ExternalId = objectId.ToString(),
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return (false, null, string.Join(",", createResult.Errors.Select(e => e.Description)), null);

                await _userManager.AddToRoleAsync(user, "Member");
            }

            if (!user.IsActive)
                return (false, null, "User is inactive", null);

            // Generate internal JWT + Refresh
            var jwtResult = await _jwtTokenService.GenerateJwtTokenAsync(user);
            var refresh = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = jwtResult.Token,
                RefreshToken = refresh,
                TokenExpiry = jwtResult.Expiry
            }, null, null);
        }
    }
}
