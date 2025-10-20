using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureAuthController : ControllerBase
    {
        private readonly IAzureAuthService _azureAuthService;

        public AzureAuthController(IAzureAuthService azureAuthService)
        {
            _azureAuthService = azureAuthService;
        }

        [HttpPost("exchange-token")]
        public async Task<IActionResult> ExchangeToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return BadRequest("Missing or invalid Authorization header");

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var result = await _azureAuthService.ExchangeAzureTokenAsync(token);

            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(result.Response);
        }
    }
}
