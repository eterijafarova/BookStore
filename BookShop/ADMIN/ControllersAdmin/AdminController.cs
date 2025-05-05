// using BookShop.ADMIN.ServicesAdmin.AdminServices;
// using BookShop.Auth.ModelsAuth;
// using BookShop.Data.Models;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Threading.Tasks;
//
// namespace BookShop.ADMIN.ControllersAdmin
// {
//     [ApiController]
//     [Route("api/v1/[controller]")]
//     public class AdminController : ControllerBase
//     {
//         private readonly IAdminService _adminService;
//         private readonly UserManager<User> _userManager; // Добавлен UserManager для работы с пользователями
//
//         public AdminController(IAdminService adminService, UserManager<User> userManager)
//         {
//             _adminService = adminService;
//             _userManager = userManager; // Инициализация UserManager
//         }
//
//         // Удалить пользователя
//         [HttpDelete("DeleteUser/{userId}")]
//         [Authorize(Roles = "Admin, SuperAdmin")]
//         public async Task<IActionResult> DeleteUser(Guid userId)
//         {
//             try
//             {
//                 await _adminService.DeleteUserAsync(userId);
//                 return Ok(new { message = "User deleted successfully." });
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { message = ex.Message });
//             }
//         }
//
//         // POST: api/genres/assignAdmin/{userId}
//         [HttpPost("assignAdmin/{userId}")]
//         [Authorize(Roles = "SuperAdmin")] // Только SuperAdmin может назначать администраторов
//         public async Task<IActionResult> AssignAdminRole(Guid userId)
//         {
//             // Получаем текущего пользователя
//             var currentUser = await _userManager.GetUserAsync(User);  
//
//             // Проверяем, является ли текущий пользователь суперадмином
//             if (!await _userManager.IsInRoleAsync(currentUser, "SuperAdmin"))
//             {
//                 return Unauthorized("You do not have permission to perform this action.");
//             }
//
//             var userToUpdate = await _userManager.FindByIdAsync(userId.ToString());
//             if (userToUpdate == null)
//             {
//                 return NotFound("User not found.");
//             }
//
//             // Назначаем роль "Admin"
//             var result = await _userManager.AddToRoleAsync(userToUpdate, "Admin");
//             if (!result.Succeeded)
//             {
//                 return BadRequest("Failed to assign Admin role.");
//             }
//
//             return Ok("User assigned as Admin.");
//         }
//
//         // POST: api/genres/removeAdmin/{userId}
//         [HttpPost("removeAdmin/{userId}")]
//         [Authorize(Roles = "SuperAdmin")] // Только SuperAdmin может удалять администраторов
//         public async Task<IActionResult> RemoveAdminRole(Guid userId)
//         {
//             // Получаем текущего пользователя
//             var currentUser = await _userManager.GetUserAsync(User);
//
//             // Проверяем, является ли текущий пользователь суперадмином
//             if (!await _userManager.IsInRoleAsync(currentUser, "SuperAdmin"))
//             {
//                 return Unauthorized("You do not have permission to perform this action.");
//             }
//
//             var userToUpdate = await _userManager.FindByIdAsync(userId.ToString());
//             if (userToUpdate == null)
//             {
//                 return NotFound("User not found.");
//             }
//
//             // Убираем роль "Admin"
//             var result = await _userManager.RemoveFromRoleAsync(userToUpdate, "Admin");
//             if (!result.Succeeded)
//             {
//                 return BadRequest("Failed to remove Admin role.");
//             }
//
//             return Ok("Admin role removed from user.");
//         }
//
//         // Обновить количество на складе
//         [HttpPost("UpdateStock/{bookId}")]
//         [Authorize(Roles = "Admin, SuperAdmin")]
//         public async Task<IActionResult> UpdateStock(int bookId, [FromBody] int quantity)
//         {
//             try
//             {
//                 await _adminService.UpdateStockAsync(bookId, quantity);
//                 return Ok(new { message = "Stock updated successfully." });
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { message = ex.Message });
//             }
//         }
//
//         // Удалить комментарий
//         [HttpDelete("DeleteComment/{commentId}")]
//         [Authorize(Roles = "Admin, SuperAdmin")]
//         public async Task<IActionResult> DeleteComment(int commentId)
//         {
//             try
//             {
//                 await _adminService.DeleteCommentAsync(commentId);
//                 return Ok(new { message = "Comment deleted successfully." });
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { message = ex.Message });
//             }
//         }
//
//         // Изменить статус заказа
//         [HttpPost("ChangeOrderStatus/{orderId}")]
//         [Authorize(Roles = "Admin, SuperAdmin")]
//         public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromBody] Order.OrderStatus newStatus)
//         {
//             try
//             {
//                 await _adminService.ChangeOrderStatusAsync(orderId, newStatus);
//                 return Ok(new { message = "Order status updated successfully." });
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { message = ex.Message });
//             }
//         }
//
//         // Изменить статус предзаказа
//         // [HttpPost("ChangePreOrderStatus/{preOrderId}")]
//         // [Authorize(Roles = "Admin, SuperAdmin")]
//         // public async Task<IActionResult> ChangePreOrderStatus(int preOrderId, [FromBody] Order.OrderStatus newStatus)
//         // {
//         //     try
//         //     {
//         //         await _adminService.ChangePreOrderStatusAsync(preOrderId, newStatus);
//         //         return Ok(new { message = "Pre-order status updated successfully." });
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         return BadRequest(new { message = ex.Message });
//         //     }
//         // }
//     }
// }
