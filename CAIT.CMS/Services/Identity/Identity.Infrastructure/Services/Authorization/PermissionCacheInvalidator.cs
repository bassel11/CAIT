using BuildingBlocks.Contracts.SecurityEvents;
using Identity.Application.Interfaces.Authorization;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Authorization
{
    public class PermissionCacheInvalidator : IPermissionCacheInvalidator
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPublishEndpoint _publishEndpoint;

        public PermissionCacheInvalidator(
                ApplicationDbContext context,
                IPermissionChecker permissionChecker,
                IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _permissionChecker = permissionChecker;
            _publishEndpoint = publishEndpoint;
        }

        public async Task InvalidateUserPermissionsByRoleAsync(Guid roleId)
        {
            var userIds = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync();

            foreach (var userId in userIds)
            {
                await PublishAndInvalidateLocal(userId);
            }

            //foreach (var userId in userIds)
            //    _permissionChecker.InvalidateCache(userId);
        }

        public async Task InvalidateUserPermissionsByUserAsync(Guid userId)
        {
            await PublishAndInvalidateLocal(userId);
        }


        private async Task PublishAndInvalidateLocal(Guid userId)
        {
            // 1. تنظيف الكاش المحلي (داخل Identity Service)
            _permissionChecker.InvalidateCache(userId);

            // 2. نشر الحدث للخدمات الخارجية (Task, Committee, etc.)
            // ملاحظة: سيتم إضافة الرسالة لجدول OutboxMessages هنا
            // ولن ترسل فعلياً إلا عند استدعاء SaveChanges في السيرفس المستدعي
            await _publishEndpoint.Publish(new UserPermissionsChangedIntegrationEvent
            {
                UserId = userId,
                OccurredAt = DateTime.UtcNow
            });
        }

        //public async Task InvalidateUserPermissionsByUserAsync(Guid userId)
        //{
        //    _permissionChecker.InvalidateCache(userId);
        //    await Task.CompletedTask;
        //}
    }
}
