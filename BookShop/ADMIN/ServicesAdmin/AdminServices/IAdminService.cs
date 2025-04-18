using BookShop.ADMIN.DTOs.DTOAdmin;
using BookShop.Data.Models;

namespace BookShop.ADMIN.ServicesAdmin.AdminServices;

public interface IAdminService
{
    Task DeleteUserAsync(Guid userId);
    Task AssignAdminRoleAsync(Guid userId);
    Task RemoveAdminRoleAsync(Guid userId);
    Task AddBookAsync(Book book);
    Task UpdateStockAsync(int bookId, int quantity);
    Task DeleteCommentAsync(int commentId);
    Task ChangeOrderStatusAsync(int orderId, Order.OrderStatus newStatus);

}
