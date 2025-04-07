using BookShop.Data;
using BookShop.Data.Models;
using BookShop.ADMIN.DTOs;
using BookShop.Auth.DTOAuth.Responses;
using Microsoft.EntityFrameworkCore;
using BookShop.Services.Interfaces;

namespace BookShop.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;

        public UserService(LibraryContext context)
        {
            _context = context;
        }

        // Получить список всех пользователей
        public async Task<List<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        // Получить пользователя по ID
        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            return user;
        }

        // Удалить пользователя по ID
        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}