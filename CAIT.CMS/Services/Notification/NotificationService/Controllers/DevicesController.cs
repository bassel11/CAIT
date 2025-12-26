using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Entities;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly NotificationDbContext _context;

        public DevicesController(NotificationDbContext context)
        {
            _context = context;
        }

        public record RegisterDeviceDto(Guid UserId, string DeviceToken, string Platform);

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto dto)
        {
            var existingDevice = await _context.UserDeviceTokens
                .FirstOrDefaultAsync(d => d.DeviceToken == dto.DeviceToken && d.UserId == dto.UserId);

            if (existingDevice != null)
            {
                // التوكن موجود، نحدث تاريخ آخر ظهور فقط
                existingDevice.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                // جهاز جديد
                _context.UserDeviceTokens.Add(new UserDeviceToken
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    DeviceToken = dto.DeviceToken,
                    Platform = dto.Platform,
                    LastUpdated = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Device registered successfully" });
        }
    }
}