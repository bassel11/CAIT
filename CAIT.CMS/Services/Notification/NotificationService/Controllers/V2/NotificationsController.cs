using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;

namespace NotificationService.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize]
    public class NotificationsController : BaseApiController
    {
        private readonly NotificationDbContext _context;

        public NotificationsController(NotificationDbContext context)
        {
            _context = context;
        }

        // 1. Fetch History (Scenario: User opens app after being offline)
        [HttpGet("{userId}")]
        [Authorize(Policy = "Permission:Notification.View")]
        public async Task<IActionResult> GetMyNotifications(Guid userId)
        {
            var result = await _context.AppNotifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt) // Newest first
                .Take(20) // Limit to last 20
                .ToListAsync();

            return Success(result, "Notifications Retrieved Successfully");
        }

        // 2. Get Unread Count (For the red badge on load)
        [HttpGet("{userId}/unread-count")]
        [Authorize(Policy = "Permission:Notification.View")]
        public async Task<IActionResult> GetUnreadCount(Guid userId)
        {
            var count = await _context.AppNotifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return Success(count, "Count Retrieved Successfully");
        }

        // 3. Mark as Read (Scenario: User clicks a notification)
        [HttpPut("{id}/read")]
        [Authorize(Policy = "Permission:Notification.Update")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _context.AppNotifications
                .Where(n => n.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

            return Success("NotificationMarkedAsRead");
        }

        // 4. Mark All as Read (Scenario: User clicks "Clear All")
        [HttpPut("{userId}/read-all")]
        [Authorize(Policy = "Permission:Notification.Update")]
        public async Task<IActionResult> MarkAllAsRead(Guid userId)
        {
            await _context.AppNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

            return Success("AllNotificationsMarkedAsRead");
        }
    }

}
