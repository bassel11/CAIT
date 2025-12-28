using BuildingBlocks.Contracts.SecurityEvents;
using Identity.Application.Security;
using Identity.Application.Security.SecurityEventPublisher;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Security
{
    public class LoginSecurityService : ILoginSecurityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISecurityEventPublisher _eventPublisher;
        private readonly ApplicationDbContext _dbContext;

        private const int FAILED_LOGIN_THRESHOLD = 3;

        public LoginSecurityService(
            UserManager<ApplicationUser> userManager,
            ISecurityEventPublisher eventPublisher,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
            _dbContext = dbContext;
        }

        public async Task HandleFailedLoginAsync(ApplicationUser user, string ipAddress)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                user.AccessFailedCount += 1;
                await _userManager.UpdateAsync(user); // سيحفظ المستخدم

                if (user.AccessFailedCount >= FAILED_LOGIN_THRESHOLD)
                {
                    await _eventPublisher.PublishAsync(
                        new FailedLoginAttemptEvent
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            FailedCount = user.AccessFailedCount,
                            IpAddress = ipAddress,
                            ThresholdExceeded = true
                        });

                    // حفظ الـ Outbox Message
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync(); // اعتماد الكل
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task HandleSuccessfulLoginAsync(ApplicationUser user, string ipAddress)
        {
            user.AccessFailedCount = 0;
            await _userManager.UpdateAsync(user);

            //await _eventPublisher.PublishAsync(
            //    new SuccessfulLoginEvent
            //    {
            //        UserId = user.Id,
            //        UserName = user.UserName,
            //        IpAddress = ipAddress,
            //        Severity = "Low"
            //    });
        }
    }

}
