using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using BookShop.ADMIN.DTOs.AdressDto;
using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;

namespace BookShop.Services.Implementations
{
    public class AdressService : IAdressService
    {
        private readonly LibraryContext _context;

        public AdressService(LibraryContext context)
        {
            _context = context;
        }
        
        public async Task<AdressResponseDto> GetAdressByIdAsync(Guid userId)  
        {
            var adress = await _context.Adresses
                .FirstOrDefaultAsync(a => a.UserId == userId); 

            if (adress == null)
            {
                return null;  
            }

            return new AdressResponseDto
            {
                Id = adress.Id,
                UserId = adress.UserId,
                Street = adress.Street,
                City = adress.City,
                State = adress.State,
                PostalCode = adress.PostalCode,
                Country = adress.Country
            };
        }
        
        public async Task<AdressResponseDto> AddAdressAsync(AdressRequestDto adressRequest)
        {
            var adress = new Adress
            {
                UserId = adressRequest.UserId, 
                Street = adressRequest.Street,
                City = adressRequest.City,
                State = adressRequest.State,
                PostalCode = adressRequest.PostalCode,
                Country = adressRequest.Country
            };

            _context.Adresses.Add(adress);
            await _context.SaveChangesAsync();

            return new AdressResponseDto
            {
                Id = adress.Id,
                UserId = adress.UserId,
                Street = adress.Street,
                City = adress.City,
                State = adress.State,
                PostalCode = adress.PostalCode,
                Country = adress.Country
            };
        }
        
        public async Task<bool> UpdateAdressAsync(Guid userId, AdressRequestDto adressRequest) 
        {
            var adress = await _context.Adresses
                .FirstOrDefaultAsync(a => a.UserId == userId); 

            if (adress == null)
            {
                return false;  
            }

            adress.Street = adressRequest.Street;
            adress.City = adressRequest.City;
            adress.State = adressRequest.State;
            adress.PostalCode = adressRequest.PostalCode;
            adress.Country = adressRequest.Country;

            await _context.SaveChangesAsync();
            return true;  
        }

        public async Task<bool> DeleteAdressAsync(Guid userId)  
        {
            var adress = await _context.Adresses
                .FirstOrDefaultAsync(a => a.UserId == userId); 

            if (adress == null)
            {
                return false;  
            }

            _context.Adresses.Remove(adress);
            await _context.SaveChangesAsync();
            return true;  
        }
    }
}
