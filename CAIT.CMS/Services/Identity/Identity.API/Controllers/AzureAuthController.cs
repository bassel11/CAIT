using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureAuthController : ControllerBase
    {
        private readonly IAzureAuthService _azureAuthService;
        private readonly IAzureB2BService _azureB2BService;

        public AzureAuthController(IAzureAuthService azureAuthService, IAzureB2BService azureB2BService)
        {
            _azureAuthService = azureAuthService;
            _azureB2BService = azureB2BService;
        }

        [HttpPost("exchange-tokenAzureAD")]
        public async Task<IActionResult> ExchangeTokenAzureAD()
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

        [HttpPost("exchange-tokenB2BGuest")]
        public async Task<IActionResult> ExchangeTokenB2BGuest()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return BadRequest("Missing or invalid Authorization header");

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var result = await _azureB2BService.ExchangeB2BTokenAsync(token);

            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(result.Response);
        }

    }
}
