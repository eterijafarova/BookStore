using BookShop.ADMIN.DTOs.CardDto;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateCardResponseDto>> Create([FromBody] CreateCardRequestDto request)
        {
            try
            {
                var dto = await _cardService.AddCardAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CardResponseDto>> GetById(Guid id)
        {
            var dto = await _cardService.GetCardByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCardRequestDto request)
        {
            var ok = await _cardService.UpdateCardAsync(request);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _cardService.DeleteCardAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}