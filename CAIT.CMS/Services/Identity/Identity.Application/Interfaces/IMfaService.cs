using Identity.Application.DTOs;

namespace Identity.Application.Interfaces
{
    public interface IMfaService
    {
        Task<(bool Success, LoginResponseDto? Response, string? Error)> VerifyMfaAsync(VerifyMfaDto dto);
    }
}
