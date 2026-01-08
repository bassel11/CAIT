using Microsoft.AspNetCore.Mvc;

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
    }
}