using BookShop.ADMIN.DTOs.PublisherDto;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/v1/publishers")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _service;

        public PublishersController(IPublisherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PublisherDto>> Create(CreatePublisherDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result == null)
                return BadRequest("Publisher with this name already exists.");
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePublisherDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}