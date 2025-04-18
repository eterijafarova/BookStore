using BookShop.Auth.ModelsAuth;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.ADMIN.ServicesAdmin.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly LibraryContext _context;

        public AdminService(LibraryContext context)
        {
            _context = context;
        }

        public async Task DeleteUserAsync(Guid userId)  // Change userId to Guid
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            // Prevent deletion of super admins
            var isSuperAdmin = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.Role.RoleName == "SuperAdmin");

            if (isSuperAdmin) throw new Exception("Cannot delete SuperAdmin.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task AssignAdminRoleAsync(Guid userId)  // Change userId to Guid
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if (adminRole == null) throw new Exception("Admin role not found");

            var userRole = new UserRole { UserId = user.Id, RoleId = adminRole.Id };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAdminRoleAsync(Guid userId)  // Change userId to Guid
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.Role.RoleName == "Admin");

            if (userRole == null) throw new Exception("Admin role not found for this user");

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Reviews.FindAsync(commentId);
            if (comment == null) throw new Exception("Comment not found");

            _context.Reviews.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeOrderStatusAsync(int orderId, Order.OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateStockAsync(int bookId, int quantity)
        {
            // 1. Find the book by bookId
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception("Book not found");
            }
            
            book.Stock += quantity;
            
            await _context.SaveChangesAsync();
        }

    }
}
