using BuildingBlocks.Shared.Authorization;
using FluentValidation;
using FluentValidation.Results;
using Identity.API.Controllers.Base;
using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : BaseController
    {
        private readonly Application.Interfaces.Permissions.IPermissionService _permissionService;
        private readonly IValidator<PermissionFilterDto> _validator;
        private readonly IPermissionChecker _checker;
        private readonly IPermissionSnapshotQuery _query;

        public PermissionController(
            Application.Interfaces.Permissions.IPermissionService permissionService,
            IValidator<PermissionFilterDto> validator,
            IPermissionChecker checker,
            IPermissionSnapshotQuery query)
        {
            _permissionService = permissionService;
            _validator = validator;
            _checker = checker;
            _query = query;
        }

        [HttpGet("GetAllPermissions")]
        [Authorize(Policy = "Permission:Permission.View")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetAllAsync();
            return Ok(result);
        }

        // GET api/permissions
        [HttpGet("GetFilteredPermissions")]
        [Authorize(Policy = "Permission:Permission.View")]
        public async Task<IActionResult> Get([FromQuery] PermissionFilterDto query)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var result = await _permissionService.GetPagedAsync(query);
            return Ok(result);
        }

        [HttpGet("GetPermissionById/{id}")]
        [Authorize(Policy = "Permission:Permission.View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _permissionService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreatePermission")]
        [Authorize(Policy = "Permission:Permission.Create")]
        public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto)
        {
            var result = await _permissionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("UpdatePermission/{id}")]
        [Authorize(Policy = "Permission:Permission.Update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionDto dto)
        {
            var result = await _permissionService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("DeletePermission/{id}")]
        [Authorize(Policy = "Permission:Permission.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _permissionService.DeleteAsync(id);
            return Ok(result);
        }


        // for calling ckeck permissions from another Service Endpoints

        [HttpGet("check")]
        [AllowAnonymous]
        // for internally between Services // please check allowaAnonymous
        public async Task<IActionResult> Check(Guid userId, string permission, Guid? resourceId = null, Guid? parentResourceId = null)
        {
            bool has = await _checker.HasPermissionAsync(userId, permission, resourceId, parentResourceId);
            return Ok(new { allowed = has });
        }

        //[HttpGet("snapshot")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Snapshot(Guid userId)
        //{
        //    var snapshots = await _query.GetSnapshotsAsync(userId);
        //    return Ok(snapshots);
        //}

        [HttpGet("snapshot")]
        [AllowAnonymous]
        public async Task<ActionResult<PermissionSnapshot>> Snapshot([FromQuery] Guid userId)
        {
            // 1. جلب البيانات الداخلية (Internal DTOs List)
            var internalList = await _query.GetSnapshotsAsync(userId);

            // 2. تجهيز كائن الاستجابة المشترك
            var sharedSnapshot = new PermissionSnapshot
            {
                UserId = userId,
                GeneratedAt = DateTime.UtcNow,
                Permissions = new List<PermissionEntry>()
            };

            // 3. عملية التحويل (Mapping) من Internal إلى Shared
            // هذا يحل مشكلة الـ JsonException لأننا نعبئ القائمة ونرسل الكائن الحاوي
            if (internalList != null && internalList.Any())
            {
                sharedSnapshot.Permissions = internalList.Select(x => new PermissionEntry
                {
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Allow = x.Allow,

                    // تحويل الـ Enums (Casting) لأن الأرقام متطابقة
                    //Scope = (ScopeType)(int)x.Scope,
                    ScopeName = x.ScopeName,

                    //ResourceType = x.ResourceType.HasValue ? (ResourceType)(int)x.ResourceType.Value : null,
                    ResourceTypeName = x.ResourceTypeName,
                    ResourceId = x.ResourceId,

                    //ParentResourceType = x.ParentResourceType.HasValue ? (ResourceType)(int)x.ParentResourceType.Value : null,
                    ParentResourceTypeName = x.ParentResourceTypeName,
                    ParentResourceId = x.ParentResourceId

                }).ToList();
            }

            // 4. إرجاع 200 OK مع الكائن
            return Ok(sharedSnapshot);
        }



    }
}
