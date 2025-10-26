using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin", AuthenticationSchemes = "BearerPolicy")]
    public class BaseController : ControllerBase
    {
    }
}
