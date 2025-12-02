using MeetingApplication.Integrations;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugRabbitController : ControllerBase
    {
        private readonly IMessageBus _bus;

        public DebugRabbitController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestMessage()
        {
            var message = new
            {
                Id = Guid.NewGuid(),
                Text = "Hello RabbitMQ from CMS",
                SentAt = DateTime.UtcNow
            };

            await _bus.PublishAsync("test.message", message);

            return Ok("Message sent to RabbitMQ");
        }
    }
}
