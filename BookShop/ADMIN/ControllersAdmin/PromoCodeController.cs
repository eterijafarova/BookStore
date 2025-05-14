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
        
        
        [HttpPost("Deactivate/{promoCodeId:guid}")]
        public async Task<IActionResult> DeactivatePromoCode(Guid promoCodeId)
        {
            var result = await _promoCodeService.DeactivatePromoCodeAsync(promoCodeId);
            if (!result)
                return NotFound(new { message = "Promo code not found" });
            return Ok(new { message = "Promo code deactivated successfully" });
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
        
        [HttpDelete("Delete/{promoCodeId:guid}")]
        public async Task<IActionResult> DeletePromoCode(Guid promoCodeId)
        {
            var result = await _promoCodeService.DeletePromoCodeAsync(promoCodeId);
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
