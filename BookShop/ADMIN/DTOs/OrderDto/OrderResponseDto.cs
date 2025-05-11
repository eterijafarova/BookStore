using System;
using System.Collections.Generic;

namespace BookShop.ADMIN.DTOs.OrderDto
{
    public class OrderResponseDto
    {
        public OrderResponseDto(Guid id, Guid userId, decimal totalPrice, string status, DateTime orderDate, List<OrderItemResponseDto> orderItems)
        {
            Id = id;
            UserId = userId;
            TotalPrice = totalPrice;
            Status = status;
            OrderDate = orderDate;
            OrderItems = orderItems;
        }

        public Guid Id { get; set; } 
        public Guid UserId { get; set; }  
        public decimal TotalPrice { get; set; }  
        public string Status { get; set; }  
        public DateTime OrderDate { get; set; } 

       
        public List<OrderItemResponseDto> OrderItems { get; set; }
    }

    public class OrderItemResponseDto
    {
        public Guid BookId { get; set; }  
        public int Quantity { get; set; } 
        public decimal Price { get; set; }  
    }
}