using BookShop.ADMIN.ServicesAdmin.AdminServices;
using BookShop.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "SuperAdmin, Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Назначить роль Admin пользователю по имени
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>Результат назначения роли</returns>
        [HttpPost("assign-admin-role-by-name/{userName}")]
        public async Task<IActionResult> AssignAdminRoleByName(string userName)
        {
            try
            {
                await _adminService.AssignAdminRoleByNameAsync(userName);
                return Ok(new { message = "Admin role assigned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Удалить роль Admin у пользователя по имени
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>Результат удаления роли</returns>
        [HttpPost("remove-admin-role-by-name/{userName}")]
        public async Task<IActionResult> RemoveAdminRoleByName(string userName)
        {
            try
            {
                await _adminService.RemoveAdminRoleByNameAsync(userName);
                return Ok(new { message = "Admin role removed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Изменить статус заказа
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="newStatus">Новый статус заказа</param>
        /// <returns>Результат изменения статуса</returns>
        [HttpPost("change-order-status/{orderId}")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromBody] Order.OrderStatus newStatus)
        {
            try
            {
                await _adminService.ChangeOrderStatusAsync(orderId, newStatus);
                return Ok(new { message = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновить количество книг на складе
        /// </summary>
        /// <param name="bookId">Идентификатор книги</param>
        /// <param name="quantity">Количество книг для добавления (можно использовать отрицательное значение для уменьшения)</param>
        /// <returns>Результат обновления склада</returns>
        [HttpPost("update-stock/{bookId}")]
        public async Task<IActionResult> UpdateStock(int bookId, [FromBody] int quantity)
        {
            try
            {
                await _adminService.UpdateStockAsync(bookId, quantity);
                return Ok(new { message = "Stock updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

            /// <summary>
            /// Получить всех пользователей
            /// </summary>
            /// <returns>Список всех пользователей</returns>
            [HttpGet("get-all-users")]
            public async Task<IActionResult> GetAllUsers()
            {
                try
                {
                    var users = await _adminService.GetAllUsersAsync();  
                    return Ok(users);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            
            /// <summary>
            /// Удалить пользователя по имени
            /// </summary>
            /// <param name="userName">Имя пользователя</param>
            /// <returns>Результат удаления</returns>
            [HttpDelete("delete-user-by-name/{userName}")]
            public async Task<IActionResult> DeleteUserByName(string userName)
            {
                try
                {
                    await _adminService.DeleteUserByNameAsync(userName);
                    return Ok(new { message = "User deleted successfully." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
    }
}
