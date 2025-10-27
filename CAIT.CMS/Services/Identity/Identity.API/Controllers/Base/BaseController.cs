using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin", AuthenticationSchemes = "BearerPolicy")]
    //[ProducesResponseType(typeof(ProblemDetails), 401)]
    //[ProducesResponseType(typeof(ProblemDetails), 403)]

    public class BaseController : ControllerBase
    {
    }
}
