using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookShop.ADMIN.DTOs.OrderDto;
using BookShop.Data.Models;

namespace BookShop.Services.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Создает новый заказ.
        /// </summary>
        /// <param name="orderRequest">Данные запроса на создание заказа (Id пользователя и список книг с количеством).</param>
        /// <returns>DTO с информацией о созданном заказе.</returns>
        Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequest);

        /// <summary>
        /// Возвращает заказ по его идентификатору.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <returns>DTO с информацией о заказе или null, если не найден.</returns>
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId);

        /// <summary>
        /// Возвращает все заказы конкретного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список DTO с информацией о заказах пользователя.</returns>
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(Guid userId);

        /// <summary>
        /// Удаляет заказ по его идентификатору.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <returns>True, если удаление прошло успешно; иначе — false.</returns>
        Task<bool> DeleteOrderAsync(Guid orderId);

        /// <summary>
        /// Обновляет статус заказа.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <param name="status">Новый статус заказа.</param>
        /// <returns>True, если обновление прошло успешно; иначе — false.</returns>
        Task<bool> UpdateOrderStatusAsync(Guid orderId, Order.OrderStatus status);
        
        
        /// <summary>
        /// Возвращает все заказы в системе.
        /// </summary>
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();

    }
}