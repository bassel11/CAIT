using Identity.Application.DTOs;

namespace Identity.Application.Interfaces
{
    public interface IAzureB2BService
    {
        Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> ExchangeB2BTokenAsync(string azureAccessToken);
    }
}
