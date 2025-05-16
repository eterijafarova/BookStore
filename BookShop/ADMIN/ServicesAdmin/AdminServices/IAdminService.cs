using BookShop.ADMIN.DTOs;
using BookShop.Auth.ModelsAuth;
using BookShop.Data.Models;

namespace BookShop.ADMIN.ServicesAdmin.AdminServices;

public interface IAdminService
{
    Task DeleteUserByNameAsync(string userName);
    Task AssignAdminRoleByNameAsync(string userName);
    Task RemoveAdminRoleByNameAsync(string userName);
    
    Task DeleteCommentAsync(int commentId);
    Task ChangeOrderStatusAsync(int orderId, Order.OrderStatus newStatus);
    Task UpdateStockAsync(int bookId, int quantity);
    Task<IEnumerable<UsersGetDto>> GetAllUsersAsync();
}