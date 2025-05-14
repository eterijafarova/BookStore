namespace BookShop.ADMIN.DTOs.OrderDto
{
    public class OrderRequestDto
    {
        public Guid UserId { get; set; }
        
        public Guid UserAddressId { get; set; }   
        public Guid UserBankCardId { get; set; }  

        /// <summary>
        /// Ваш промо-код (опционально)
        /// </summary>
        public string? PromoCode { get; set; }

        public List<OrderItemRequestDto> OrderItems { get; set; } 
            = new List<OrderItemRequestDto>();
    }

    public class OrderItemRequestDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}