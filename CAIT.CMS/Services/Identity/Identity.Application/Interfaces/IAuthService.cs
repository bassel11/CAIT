using Identity.Application.DTOs;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, LoginResponseDto? Response, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> LoginAsync(LoginDto dto);
        Task<(bool Success, string? Error)> LogoutAsync(string refreshToken);
        Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string token, string refreshToken);
        Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        //Task<(bool Success, string? Message, string? Error)> ForgotPasswordAsync(string email);
        Task<(bool Success, string? Message, string? ResetLink, string? Token)> ForgotPasswordAsync(string email);
        Task<(bool Success, string? Message, string? Error)> ResetPasswordAsync(ResetPasswordDto dto);

        Task<(bool Success, string? Message, string? Error)> ForceResetPasswordAsync(ForceResetPasswordDto dto);

    }
}
