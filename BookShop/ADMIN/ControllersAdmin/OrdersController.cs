using BookShop.ADMIN.DTOs.OrderDto;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Создать новый заказ.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create([FromBody] OrderRequestDto request)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { orderId = result.Id },
                    result
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить заказ по его Id.
        /// </summary>
        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(Guid orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Получить все заказы пользователя.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetByUser(Guid userId)
        {
            var results = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(results);
        }

        /// <summary>
        /// Удалить заказ.
        /// </summary>
        [HttpDelete("{orderId:guid}")]
        public async Task<IActionResult> Delete(Guid orderId)
        {
            var success = await _orderService.DeleteOrderAsync(orderId);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// DTO для изменения статуса заказа.
        /// </summary>
        public class UpdateOrderStatusDto
        {
            public Order.OrderStatus Status { get; set; }
        }

        /// <summary>
        /// Обновить статус заказа.
        /// </summary>
        [HttpPatch("{orderId:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
