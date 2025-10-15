namespace Identity.Application.Interfaces
{
    public interface ILdapAuthService
    {
        Task<(bool Success, string? ExternalId, string? Error)> AuthenticateAsync(string username, string password);
    }
}
