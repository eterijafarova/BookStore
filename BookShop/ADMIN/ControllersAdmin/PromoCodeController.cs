using BookShop.ADMIN.DTOs;
using BookShop.Data;
using BookShop.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.ADMIN.ControllersAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly LibraryContext _context;

        public PromoCodeController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/promocodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromoCodeDto>>> GetPromoCodes()
        {
            var promoCodes = await _context.PromoCodes
                .Select(p => new PromoCodeDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Discount = p.Discount,
                    ExpiryDate = p.ExpiryDate,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return Ok(promoCodes);
        }

        // GET: api/promocodes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PromoCodeDto>> GetPromoCode(Guid id)
        {
            var promoCode = await _context.PromoCodes
                .Where(p => p.Id == id)
                .Select(p => new PromoCodeDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Discount = p.Discount,
                    ExpiryDate = p.ExpiryDate,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();

            if (promoCode == null)
            {
                return NotFound();
            }

            return Ok(promoCode);
        }

        // POST: api/promocodes
        [HttpPost]
        public async Task<ActionResult<PromoCodeDto>> CreatePromoCode([FromBody] CreatePromoCodeDto dto)
        {
            var promoCode = new PromoCode
            {
                Code = dto.Code,
                Discount = dto.Discount,
                ExpiryDate = dto.ExpiryDate,
                IsActive = dto.IsActive
            };

            _context.PromoCodes.Add(promoCode);
            await _context.SaveChangesAsync();

            var createdPromoCodeDto = new PromoCodeDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                Discount = promoCode.Discount,
                ExpiryDate = promoCode.ExpiryDate,
                IsActive = promoCode.IsActive
            };

            return CreatedAtAction(nameof(GetPromoCode), new { id = promoCode.Id }, createdPromoCodeDto);
        }

        // PUT: api/promocodes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromoCode(Guid id, [FromBody] UpdatePromoCodeDto dto)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);

            if (promoCode == null)
            {
                return NotFound();
            }

            promoCode.Code = dto.Code;
            promoCode.Discount = dto.Discount;
            promoCode.ExpiryDate = dto.ExpiryDate;
            promoCode.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/promocodes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromoCode(Guid id)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);
            if (promoCode == null)
            {
                return NotFound();
            }

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
