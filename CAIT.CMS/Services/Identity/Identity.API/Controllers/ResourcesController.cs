using Identity.API.Controllers.Base;
using Identity.Application.DTOs.Resources;
using Identity.Application.Interfaces.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : BaseController
    {
        #region
        private readonly IResourceService _resourceService;
        #endregion

        #region Constructors
        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        #endregion


        #region Actions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resources = await _resourceService.GetAllResourcesAsync();
            return Ok(resources);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var resource = await _resourceService.GetResourceByIdAsync(id);
            if (resource == null) return NotFound();
            return Ok(resource);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceCreateDto dto)
        {
            var userId = Guid.NewGuid(); // Replace with actual current user ID
            var created = await _resourceService.CreateResourceAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ResourceUpdateDto dto)
        {
            try
            {
                var updated = await _resourceService.UpdateResourceAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _resourceService.DeleteResourceAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        #endregion

    }
}
