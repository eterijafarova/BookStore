using BookShop.ADMIN.DTOs.AdressDto;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdressController : ControllerBase
    {
        private readonly IAdressService _adressService;

        public AdressController(IAdressService adressService)
        {
            _adressService = adressService;
        }
        
        
        [HttpPost]
        public async Task<IActionResult> AddAdress([FromBody] AdressRequestDto adressRequest)
        {
            var result = await _adressService.AddAdressAsync(adressRequest);
            return Ok(result);
        }

        
        [HttpGet("{guid}")]
        public async Task<IActionResult> GetAdressByIdAsync(Guid guid) 
        {
            var adress = await _adressService.GetAdressByIdAsync(guid); 
            if (adress == null)
            {
                return NotFound("Address not found.");
            }

            return Ok(adress);
        }

       
        [HttpPut("{guid}")]
        public async Task<IActionResult> UpdateAdressAsync(Guid guid, [FromBody] AdressRequestDto adressRequest) 
        {
            var isUpdated = await _adressService.UpdateAdressAsync(guid, adressRequest); 
            if (!isUpdated)
            {
                return NotFound("Address not found.");
            }

            return NoContent();
        }

  
        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteAdressAsync(Guid guid) 
        {
            var isDeleted = await _adressService.DeleteAdressAsync(guid);  
            if (!isDeleted)
            {
                return NotFound("Address not found.");
            }

            return NoContent();
        }
    }
}
