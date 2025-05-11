using BookShop.Data.Models;

namespace BookShop.ADMIN.DTOs.OrderDto;

public class UpdateOrderStatusRequestDto
{
    public Order.OrderStatus Status { get; set; }
}
