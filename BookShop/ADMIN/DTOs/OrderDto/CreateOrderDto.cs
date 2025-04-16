namespace BookShop.ADMIN.DTOs.OrderDto
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemDto>? OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
