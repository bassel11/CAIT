using Identity.Core.Entities;

namespace Identity.Application.Security
{
    public interface ILoginSecurityService
    {
        Task HandleFailedLoginAsync(ApplicationUser user, string ipAddress);
        Task HandleSuccessfulLoginAsync(ApplicationUser user, string ipAddress);
    }
}
