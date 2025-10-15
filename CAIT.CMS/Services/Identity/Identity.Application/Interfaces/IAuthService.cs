using Identity.Application.DTOs;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, LoginResponseDto? Response, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, LoginResponseDto? Response, string? Error)> LoginAsync(LoginDto dto);
        Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string token, string refreshToken);

        // خاصة ب LDAP 
        //Task<(bool Success, string? ExternalId, string? Error)> LoginWithLdapAsync(string username, string password);
    }
}
