using BookShop.ADMIN.ServicesAdmin.AdminServices;
using BookShop.Data.Models;
using Microsoft.AspNetCore.Authorization;
namespace BookShop.ADMIN.ControllersAdmin;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // Удалить пользователя
    [HttpDelete("DeleteUser/{userId}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            await _adminService.DeleteUserAsync(userId);
            return Ok(new { message = "User deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Назначить роль админа
    [HttpPost("AssignAdmin/{userId}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AssignAdmin(Guid userId)
    {
        try
        {
            await _adminService.AssignAdminRoleAsync(userId);
            return Ok(new { message = "Admin role assigned successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Удалить роль админа
    [HttpDelete("RemoveAdmin/{userId}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveAdmin(Guid userId)
    {
        try
        {
            await _adminService.RemoveAdminRoleAsync(userId);
            return Ok(new { message = "Admin role removed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Добавить книгу
    [HttpPost("AddBook")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        try
        {
            await _adminService.AddBookAsync(book);
            return Ok(new { message = "Book added successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Обновить количество на складе
    [HttpPost("UpdateStock/{bookId}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
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

    // Удалить комментарий
    [HttpDelete("DeleteComment/{commentId}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        try
        {
            await _adminService.DeleteCommentAsync(commentId);
            return Ok(new { message = "Comment deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Изменить статус заказа
    [HttpPost("ChangeOrderStatus/{orderId}")]
    [Authorize(Roles = "Admin, SuperAdmin")]
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

    // Изменить статус предзаказа
    //?
}
