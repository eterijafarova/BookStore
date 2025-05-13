using BookShop.ADMIN.DTOs;

namespace BookShop.Services.Interfaces
{
    public interface IPromoCodeService
    {
        /// <summary>
        /// Создаёт новый промокод.
        /// </summary>
        Task<PromoCodeResponseDto> CreatePromoCodeAsync(CreatePromoCodeDTO dto);

        /// <summary>
        /// Получает промокод по его тексту (или null, если не найден).
        /// </summary>
        Task<PromoCodeResponseDto?> GetPromoCodeAsync(string code);

        /// <summary>
        /// Проверяет и «применяет» промокод для указанного пользователя.
        /// </summary>
        Task<bool> ApplyPromoCodeAsync(string code, Guid userId);

        /// <summary>
        /// Деактивирует промокод, устанавливая IsActive = false.
        /// </summary>
        Task<bool> DeactivatePromoCodeAsync(Guid promoCodeId);

        /// <summary>
        /// Удаляет промокод из базы данных.
        /// </summary>
        Task<bool> DeletePromoCodeAsync(Guid promoCodeId);
    }
}