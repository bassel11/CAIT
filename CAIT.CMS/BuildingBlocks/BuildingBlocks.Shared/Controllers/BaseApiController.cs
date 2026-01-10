using BuildingBlocks.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Shared.Controllers
{
    /// <summary>
    /// Base Controller that provides access to contextual resource IDs extracted by Middleware.
    /// All your Microservices Controllers should inherit from this.
    /// </summary>
    [ApiController]
    //[Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// الحصول على معرف المورد الحالي (إذا وجد)
        /// يتم استخراجه من Route {resourceId} أو Header X-ResourceId أو Query
        /// </summary>
        /// 
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        protected Guid? CurrentResourceId
        {
            get
            {
                if (HttpContext.Items.TryGetValue("ResourceId", out var idObj) && idObj is Guid id)
                {
                    return id;
                }
                return null;
            }
        }

        /// <summary>
        /// الحصول على معرف المورد الأب (للسياق الهرمي)
        /// يتم استخراجه من Query ?parentResourceId أو Header X-ParentResourceId
        /// </summary>
        protected Guid? CurrentParentResourceId
        {
            get
            {
                if (HttpContext.Items.TryGetValue("ParentResourceId", out var idObj) && idObj is Guid id)
                {
                    return id;
                }
                return null;
            }
        }

        protected IActionResult Success<T>(T data, string message = "Operation completed successfully.")
        {
            return Ok(Result<T>.Success(data, message));
        }

        protected IActionResult CreatedSuccess<T>(string actionName, object routeValues, T data, string message = "Resource created successfully.")
        {
            return CreatedAtAction(actionName, routeValues, Result<T>.Success(data, message));
        }

        protected IActionResult Success(string message = "Operation completed successfully.")
        {
            return Ok(Result.Success(message));
        }
    }
}