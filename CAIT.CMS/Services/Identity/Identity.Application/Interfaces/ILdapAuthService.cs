using Identity.Application.DTOs;

namespace Identity.Application.Interfaces
{
    public interface ILdapAuthService
    {
        Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> LoginAsync(string username, string password);
        Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string token, string refreshToken);
    }
}
