using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;

namespace Identity.Infrastructure.Services
{
    public class LdapAuthService : ILdapAuthService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public LdapAuthService(IConfiguration config,
                                ApplicationDbContext dbContext,
                                UserManager<ApplicationUser> userManager,
                                IJwtTokenService jwtTokenService,
                                IRefreshTokenService refreshTokenService)
        {
            _config = config;
            _dbContext = dbContext;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> LoginAsync(string username, string password)
        {
            // إعداد LDAP من الإعدادات
            var ldapConfig = _config.GetSection("Ldap");
            string url = ldapConfig["Url"]!;
            string bindDn = ldapConfig["BindDn"]!;
            string bindPassword = ldapConfig["BindPassword"]!;
            string baseDn = ldapConfig["BaseDn"]!;
            string userFilter = ldapConfig["UserFilter"] ?? "(sAMAccountName={0})";
            //string domain = ldapConfig["Domain"] ?? "UNKNOWN";

            try
            {
                var uri = new Uri(url);

                using var conn = new LdapConnection
                {
                    SecureSocketLayer = uri.Scheme.Equals("ldaps", StringComparison.OrdinalIgnoreCase)
                };

                await conn.ConnectAsync(uri.Host, uri.Port);
                await conn.BindAsync(bindDn, bindPassword);

                var filter = string.Format(userFilter, username);

                var searchResults = await conn.SearchAsync(
                    baseDn,
                    LdapConnection.ScopeSub,
                    filter,
                    new[] { "dn", "sAMAccountName", "objectGUID", "givenName", "sn", "mail" },
                    false
                );

                LdapEntry? entry = null;
                await foreach (var result in searchResults)
                {
                    entry = result;
                    break;
                }

                if (entry == null)
                    return (false, null, "User not found in Active Directory", null);

                var userDn = entry.Dn;

                // تحقق من كلمة المرور
                try
                {
                    using var userConn = new LdapConnection
                    {
                        SecureSocketLayer = uri.Scheme.Equals("ldaps", StringComparison.OrdinalIgnoreCase)
                    };

                    await userConn.ConnectAsync(uri.Host, uri.Port);
                    await userConn.BindAsync(userDn, password);
                }
                catch
                {
                    return (false, null, "Invalid credentials", null);
                }

                // for domain name
                var dcParts = userDn.Split(',')
                                    .Where(x => x.StartsWith("DC=", StringComparison.OrdinalIgnoreCase))
                                    .Select(x => x.Substring(3));
                string domain = string.Join(".", dcParts);

                // استخراج بيانات المستخدم
                var attributes = entry.GetAttributeSet();
                string? externalId = null;
                if (attributes.ContainsKey("objectGUID"))
                    externalId = BitConverter.ToString(attributes["objectGUID"].ByteValue).Replace("-", "");
                else if (attributes.ContainsKey("sAMAccountName"))
                    externalId = attributes["sAMAccountName"].StringValue;
                else
                    externalId = username;

                string? email = attributes.ContainsKey("mail") ? attributes["mail"].StringValue : $"{username}@{domain}";
                string? firstName = attributes.ContainsKey("givenName") ? attributes["givenName"].StringValue : username;
                string? lastName = attributes.ContainsKey("sn") ? attributes["sn"].StringValue : "";

                // التحقق من وجود المستخدم في قاعدة البيانات
                var user = await _userManager.Users
                                             .FirstOrDefaultAsync(u => u.Email == email
                                             && u.AuthType == ApplicationUser.AuthenticationType.OnPremAD);

                // التحقق من صلاحية الحساب
                if (user?.ExpirationDate.HasValue == true && user.ExpirationDate < DateTime.UtcNow)
                {
                    return (false, null, "Account expired", null);
                }

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        UserType = UserType.InternalEmployee,
                        AuthType = ApplicationUser.AuthenticationType.OnPremAD,
                        AdAccount = username,
                        AdDomain = domain,
                        ExternalId = externalId,
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return (false, null, string.Join(", ", result.Errors.Select(e => e.Description)), null);

                    await _userManager.AddToRoleAsync(user, "Member");
                }

                // إنشاء التوكنات
                var jwtResult = await _jwtTokenService.GenerateJwtTokenAsync(user);
                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

                var response = new LoginResponseDto
                {
                    Token = jwtResult.Token,
                    RefreshToken = refreshToken,
                    TokenExpiry = jwtResult.Expiry
                };

                return (true, response, null, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message, null);
            }
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string token, string refreshToken)
        {
            var storedToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken && !r.Revoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return (false, null, "Invalid or expired refresh token");

            var user = storedToken.User;
            var jwtResultNew = await _jwtTokenService.GenerateJwtTokenAsync(user);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            storedToken.Revoked = true;
            await _dbContext.SaveChangesAsync();

            return (true, new LoginResponseDto
            {
                Token = jwtResultNew.Token,
                RefreshToken = newRefreshToken,
                TokenExpiry = jwtResultNew.Expiry
            }, null);
        }
    }
}
