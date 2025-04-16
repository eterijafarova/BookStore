using BookShop.ADMIN.DTOs.OrderDto;
using BookShop.Data.Models;
using BookShop.Shared.DTO.Requests;
using BookShop.Shared.DTO.Response;

namespace BookShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderResponseDto> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(int page = 1, int pageSize = 20);
        Task<bool> DeleteOrderAsync(int orderId);
        
        Task<bool> UpdateOrderStatusAsync(int orderId, Order.OrderStatus status);
    }
}