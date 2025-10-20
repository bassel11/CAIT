using Identity.Application.DTOs;

namespace Identity.Application.Interfaces
{
    public interface IAzureAuthService
    {
        Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> ExchangeAzureTokenAsync(string azureAccessToken);
    }
}
