using BookShop.ADMIN.DTOs;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromoCodeController : ControllerBase
    {
        private readonly IPromoCodeService _promoCodeService;

        public PromoCodeController(IPromoCodeService promoCodeService)
        {
            _promoCodeService = promoCodeService;
        }

       
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePromoCode([FromBody] CreatePromoCodeDTO dto)
        {
            try
            {
                var promoCode = await _promoCodeService.CreatePromoCodeAsync(dto);
                return Ok(promoCode);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    
        [HttpGet("Get/{code}")]
        public async Task<IActionResult> GetPromoCode(string code)
        {
            var promoCode = await _promoCodeService.GetPromoCodeAsync(code);
            if (promoCode == null)
                return NotFound(new { message = "Promo code not found" });
            return Ok(promoCode);
        }

        
        [HttpPost("Apply")]
        public async Task<IActionResult> ApplyPromoCode([FromBody] ApplyPromoCodeRequest request)
        {
            var isApplied = await _promoCodeService.ApplyPromoCodeAsync(request.Code, request.UserId);
            if (!isApplied)
                return BadRequest(new { message = "Invalid or expired promo code" });
            return Ok(new { message = "Promo code applied successfully" });
        }
        
        
        [HttpPost("Deactivate")]
        public async Task<IActionResult> DeactivatePromoCode(string code)
        {
            var result = await _promoCodeService.DeactivatePromoCodeAsync(code);
            if (!result)
                return NotFound(new { message = "Promo code not found" });
            return Ok(new { message = "Promo code deactivated successfully" });
        }
        
        
        /// <summary>
        /// Активировать промокод по его тексту.
        /// </summary>
        [HttpPost("Activate")]
        public async Task<IActionResult> Activate([FromQuery] string code)
        {
            var ok = await _promoCodeService.ActivatePromoCodeAsync(code);
            if (!ok)
                return NotFound(new { message = $"Promo code '{code}' not found" });

            return Ok(new { message = "Promo code activated" });
        }
        
        /// <summary>
        /// Получить все промокоды — доступно Admin и SuperAdmin
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<PromoCodeResponseDto>>> GetAll()
        {
            var list = await _promoCodeService.GetAllPromoCodesAsync();
            return Ok(list);
        }
        
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeletePromoCode(string code)
        {
            var result = await _promoCodeService.DeletePromoCodeAsync(code);
            if (!result)
                return NotFound(new { message = "Promo code not found" });
            return Ok(new { message = "Promo code deleted successfully" });
        }
    }

    
    
    public class ApplyPromoCodeRequest
    {
        public string Code { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
