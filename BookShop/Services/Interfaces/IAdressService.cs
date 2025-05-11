using BookShop.ADMIN.DTOs.AdressDto;

namespace BookShop.Services.Interfaces
{
    public interface IAdressService
    {
        Task<AdressResponseDto> GetAdressByIdAsync(Guid id);
        Task<AdressResponseDto> AddAdressAsync(AdressRequestDto adressRequest);
        Task<bool> UpdateAdressAsync(Guid id, AdressRequestDto adressRequest);
        Task<bool> DeleteAdressAsync(Guid id);
    }
}