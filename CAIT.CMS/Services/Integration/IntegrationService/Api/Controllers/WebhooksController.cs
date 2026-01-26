using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Api.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(ILogger<WebhooksController> logger)
        {
            _logger = logger;
        }

        // Microsoft ستستدعي هذا الرابط عند حدوث تغيير في التقويم
        [HttpPost("outlook")]
        public async Task<IActionResult> ReceiveOutlookNotification()
        {
            // TODO: التحقق من التنبيه ومعالجته
            // Validation Logic (Microsoft Validation Token) goes here
            _logger.LogInformation("Webhook received from Outlook.");
            return Ok();
        }
    }
}
