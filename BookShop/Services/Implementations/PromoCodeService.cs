using BookShop.ADMIN.DTOs;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Services.Implementations
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly LibraryContext _context;

        public PromoCodeService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<PromoCodeResponseDto> CreatePromoCodeAsync(CreatePromoCodeDTO dto)
        {
            var normalizedCode = dto.Code.Trim().ToLower();
            
            var exists = await _context.PromoCodes
                .AnyAsync(pc => pc.Code.ToLower() == normalizedCode);

            if (exists)
                throw new InvalidOperationException("Promo code already exists.");

            var promo = new PromoCode
            {
                Code       = dto.Code.Trim(),
                Discount   = dto.Discount,
                ExpiryDate = dto.ExpiryDate,
                IsActive   = true
            };

            _context.PromoCodes.Add(promo);
            await _context.SaveChangesAsync();

            return new PromoCodeResponseDto
            {
                Id         = promo.Id,
                Code       = promo.Code,
                Discount   = promo.Discount,
                ExpiryDate = promo.ExpiryDate,
                IsActive   = promo.IsActive
            };
        }

        public async Task<PromoCodeResponseDto?> GetPromoCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToLower();
            var promo = await _context.PromoCodes
                .FirstOrDefaultAsync(pc => pc.Code.ToLower() == normalizedCode);

            if (promo == null)
                return null;

            return new PromoCodeResponseDto
            {
                Id         = promo.Id,
                Code       = promo.Code,
                Discount   = promo.Discount,
                ExpiryDate = promo.ExpiryDate,
                IsActive   = promo.IsActive && promo.ExpiryDate > DateTime.UtcNow
            };
        }

        public async Task<bool> ApplyPromoCodeAsync(string code, Guid userId)
        {
            var normalizedCode = code.Trim().ToLower();
            var promo = await _context.PromoCodes
                .FirstOrDefaultAsync(pc => pc.Code.ToLower() == normalizedCode);

            if (promo == null || !promo.IsActive || promo.ExpiryDate <= DateTime.UtcNow)
                return false;
            
            return true;
        }

        public async Task<bool> DeactivatePromoCodeAsync(Guid promoCodeId)
        {
            var promo = await _context.PromoCodes.FindAsync(promoCodeId);
            if (promo == null) return false;

            promo.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePromoCodeAsync(Guid promoCodeId)
        {
            var promo = await _context.PromoCodes.FindAsync(promoCodeId);
            if (promo == null) return false;

            _context.PromoCodes.Remove(promo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
