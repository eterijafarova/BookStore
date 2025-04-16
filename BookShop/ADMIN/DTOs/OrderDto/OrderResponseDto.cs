namespace BookShop.ADMIN.DTOs.OrderDto
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
