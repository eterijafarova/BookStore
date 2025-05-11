using BookShop.ADMIN.DTOs.CardDto;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using BookShop.Encryptor;

namespace BookShop.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly LibraryContext _context;

        public CardService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<CreateCardResponseDto> AddCardAsync(CreateCardRequestDto request)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == request.UserId))
                throw new KeyNotFoundException($"User '{request.UserId}' not found.");

            if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.Length < 12)
                throw new ArgumentException("Invalid card number.");

            var encryptedNumber = EncryptionHelper.Encrypt(request.CardNumber);
            var encryptedCvv    = EncryptionHelper.Encrypt(request.CVV);
            var last4           = request.CardNumber[^4..];

            var card = new BankCard
            {
                CardNumber     = encryptedNumber,
                Last4Digits    = last4,
                CardHolderName = request.CardHolderName,
                ExpirationDate = request.ExpirationDate,
                CVV            = encryptedCvv,
                UserId         = request.UserId
            };

            _context.BankCards.Add(card);
            await _context.SaveChangesAsync();

            return new CreateCardResponseDto
            {
                Id             = card.Id,
                Last4Digits    = card.Last4Digits,
                CardHolderName = card.CardHolderName,
                ExpirationDate = card.ExpirationDate,
                UserId         = card.UserId
            };
        }

        public async Task<CardResponseDto> GetCardByIdAsync(Guid cardId)
        {
            var card = await _context.BankCards.FindAsync(cardId);
            if (card == null) return null;

            return new CardResponseDto
            {
                Id             = card.Id,
                Last4Digits    = card.Last4Digits,
                CardHolderName = card.CardHolderName,
                ExpirationDate = card.ExpirationDate,
                UserId         = card.UserId
            };
        }

        public async Task<bool> UpdateCardAsync(UpdateCardRequestDto request)
        {
            var card = await _context.BankCards.FindAsync(request.Id);
            if (card == null) return false;

            if (!string.IsNullOrWhiteSpace(request.CardNumber))
            {
                var num = request.CardNumber;
                card.Last4Digits = num[^4..];
                card.CardNumber  = EncryptionHelper.Encrypt(num);
            }
            if (!string.IsNullOrWhiteSpace(request.CVV))
                card.CVV = EncryptionHelper.Encrypt(request.CVV);

            card.CardHolderName = request.CardHolderName;
            card.ExpirationDate  = request.ExpirationDate ?? card.ExpirationDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCardAsync(Guid cardId)
        {
            var card = await _context.BankCards.FindAsync(cardId);
            if (card == null) return false;

            _context.BankCards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }

        public string GetBankByCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
                return "Invalid card number";

            return cardNumber[0] switch
            {
                '4' => "Capital Bank",
                '5' => "ABB Bank",
                '6' => "Leo Bank",
                '3' => "VISA",
                '7' => "MasterCard",
                _   => "Unknown"
            };
        }
    }
}
