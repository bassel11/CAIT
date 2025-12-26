using Microsoft.AspNetCore.Mvc;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/test-notify")]
    public class TestNotificationController : ControllerBase
    {
        private readonly IAppNotificationService _service;

        public TestNotificationController(IAppNotificationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Send()
        {
            // نفس الـ ID الموجود في HTML
            var userId = Guid.Parse("E383FEEA-BC94-48FB-2B85-08DE18039041");

            await _service.SendNotificationAsync(
                userId,
                "Test Message 🚀",
                "If you see this, SignalR is working!",
                "/link",
                "Success"
            );

            return Ok("Sent!");
        }
    }
}
