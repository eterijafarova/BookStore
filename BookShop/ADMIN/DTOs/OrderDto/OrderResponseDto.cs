using BookShop.Data.Models;

namespace BookShop.ADMIN.DTOs.OrderDto
{
    // DTO, который возвращает сервис после создания / получения заказа
    public class OrderResponseDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Сумма всех позиций до применения скидки.
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Промо-код, который был применён (если был).
        /// </summary>
        public string? PromoCode { get; set; }

        /// <summary>
        /// Количество денег, списанное в качестве скидки.
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Итоговая сумма к оплате (OriginalPrice – DiscountAmount).
        /// </summary>
        public decimal FinalPrice { get; set; }

        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        public Guid UserAddressId { get; set; }
        public Guid UserBankCardId { get; set; }

        public List<OrderItemResponseDto> OrderItems { get; set; }
            = new List<OrderItemResponseDto>();
    }

    public class OrderItemResponseDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }

        /// <summary>
        /// Цена за единицу товара на момент заказа.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}