using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureB2BController : ControllerBase
    {
        private readonly IAzureB2BService _azureB2BService;

        public AzureB2BController(IAzureB2BService azureB2BService)
        {
            _azureB2BService = azureB2BService;
        }

        [HttpPost("exchange-token")]
        public async Task<IActionResult> ExchangeToken()
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
