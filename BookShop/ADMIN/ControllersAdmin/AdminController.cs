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
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Назначить роль Admin пользователю по имени
        /// Доступно только Admin
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
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
        /// Доступно только Admin
        /// </summary>
        [Authorize(Roles = "SuperAdmin")]
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
        /// Доступно Admin и SuperAdmin
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin")]
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
        /// Доступно Admin и SuperAdmin
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin")]
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
        /// Доступно Admin и SuperAdmin
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin")]
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
        /// Доступно Admin и SuperAdmin
        /// </summary>
        [Authorize(Roles = "Admin,SuperAdmin")]
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
