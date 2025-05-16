using BookShop.ADMIN.DTOs;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using BookShop.Auth.ModelsAuth;

namespace BookShop.ADMIN.ServicesAdmin.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly LibraryContext _context;

        public AdminService(LibraryContext context)
        {
            _context = context;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Reviews.FindAsync(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            _context.Reviews.Remove(comment);
            await _context.SaveChangesAsync();
        }


        public async Task ChangeOrderStatusAsync(int orderId, Order.OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }


        public async Task UpdateStockAsync(int bookId, int quantity)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception("Book not found.");
            }

            book.Stock += quantity;
            await _context.SaveChangesAsync();
        }


        public async Task DeleteUserByNameAsync(string userName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new Exception("User not found.");
            }


            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRoles);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }


        public async Task AssignAdminRoleByNameAsync(string userName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Admin");

            if (role == null)
            {
                throw new Exception("Role Admin not found.");
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

            if (userRole == null)
            {
                userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }
        }


        public async Task RemoveAdminRoleByNameAsync(string userName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Admin");

            if (role == null)
            {
                throw new Exception("Role Admin not found.");
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);

                var userRoleForUser = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "User");

                if (userRoleForUser != null)
                {
                    var newUserRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = userRoleForUser.Id
                    };

                    _context.UserRoles.Add(newUserRole);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User does not have 'Admin' role.");
            }
        }

        public async Task<IEnumerable<UsersGetDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles!)
                .ThenInclude(ur => ur.Role!)
                .ToListAsync();
            
            return users.Select(u => new UsersGetDto()
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                IsEmailConfirmed = u.IsEmailConfirmed,
                Roles = u.UserRoles
                    .Select(ur => ur.Role!.RoleName)
                    .ToList()
            });
        }
    }
}
