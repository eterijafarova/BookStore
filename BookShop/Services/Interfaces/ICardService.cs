using System;
using System.Threading.Tasks;
using BookShop.ADMIN.DTOs.CardDto;

namespace BookShop.Services.Interfaces
{
    public interface ICardService
    {
        Task<CreateCardResponseDto> AddCardAsync(CreateCardRequestDto request);
        Task<CardResponseDto> GetCardByIdAsync(Guid cardId);
        Task<bool> UpdateCardAsync(UpdateCardRequestDto request);
        Task<bool> DeleteCardAsync(Guid cardId);
        string GetBankByCardNumber(string cardNumber);
    }
}