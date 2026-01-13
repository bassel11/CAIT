using BuildingBlocks.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Shared.Controllers
{

    [ApiController]
    //[Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        #region Mediator & Context

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
        #endregion

        #region Standard Responses

        // ----------------------------------------------------------------
        // 1. Success (GET / DELETE)
        // ----------------------------------------------------------------
        protected IActionResult Success<T>(T data, string message = "Operation completed successfully.")
        {
            return Ok(Result<T>.Success(data, message));
        }

        protected IActionResult Success(string message = "Operation completed successfully.")
        {
            return Ok(Result.Success(message));
        }

        // ----------------------------------------------------------------
        // 2. CreatedSuccess (POST)
        // ----------------------------------------------------------------
        protected IActionResult CreatedSuccess<T>(string actionName, object routeValues, T data, string message = "Resource created successfully.")
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                return Ok(Result<T>.Success(data, message));
            }

            return CreatedAtAction(actionName, routeValues, Result<T>.Success(data, message));
        }

        // ----------------------------------------------------------------
        // 3. EditSuccess (PUT / PATCH)
        // ----------------------------------------------------------------
        protected IActionResult EditSuccess<T>(T data, string message = "Resource updated successfully.")
        {
            return Ok(Result<T>.Success(data, message));
        }

        protected IActionResult EditSuccess(string message = "Resource updated successfully.")
        {
            return Ok(Result.Success(message));
        }

        #endregion
    }
}