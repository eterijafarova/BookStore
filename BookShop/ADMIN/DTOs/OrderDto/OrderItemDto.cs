using System;

namespace BookShop.ADMIN.DTOs.OrderDto
{
    public class OrderItemDto
    {
        public Guid BookId { get; set; } 
        public int Quantity { get; set; } 
        public decimal Price { get; set; }  

    
        public OrderItemDto(Guid bookId, int quantity, decimal price)
        {
            BookId = bookId;
            Quantity = quantity;
            Price = price;
        }
    }
}