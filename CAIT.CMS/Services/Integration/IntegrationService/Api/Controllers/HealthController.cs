using Microsoft.AspNetCore.Mvc;

namespace IntegrationService.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { Status = "Healthy", Service = "IntegrationService" });
    }
}
