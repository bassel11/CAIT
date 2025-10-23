using Identity.Application.DTOs;
using Identity.Core.Enums;

namespace Identity.Application.Interfaces
{
    public interface IMfaService
    {
        Task<(bool Success, LoginResponseDto? Response, string? Error)> VerifyMfaAsync(VerifyMfaDto dto);

        Task<(bool Success, string? QrCodeUrl, string? Error)> EnableMfaAsync(string userId, MFAMethod deliveryMethod);

        //Task<(bool Success, string? QrCodeUrl, string? Error)> EnableTfaAsync(string userId);
    }
}
