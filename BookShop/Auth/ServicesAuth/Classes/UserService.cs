 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.ADMIN.DTOs.PublisherDto;
using BookShop.Auth.ModelsAuth;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
// using BookShop.Services.Implementations;
// using Microsoft.EntityFrameworkCore;
//
// namespace BookShop.Auth.ServicesAuth.Classes
// {
//     public class UserService : IUserService
//     {
//         private readonly LibraryContext _context;
//
//         public UserService(LibraryContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<ChatService.UserDto> GetUserAsync(Guid userId)
//         {
//             var user = await _context.Users
//                 .Include(u => u.UserRoles)
//                 .ThenInclude(ur => ur.Role)
//                 .FirstOrDefaultAsync(u => u.Id == userId);
//             
//             if (user == null) return null;
//
//             return new ChatService.UserDto
//             {
//                 Id = user.Id,
//                 Email = user.Email,
//                 UserName = user.UserName,
//                 EmailConfirmed = user.IsEmailConfirmed,
//                 EmailConfirmationToken = null,
//                 EmailConfirmationTokenExpiry = null
//             };
//         }
//
//         public async Task<IEnumerable<ChatService.UserDto>> GetUsersAsync(int page, int pageSize)
//         {
//             var users = await _context.Users
//                 .Skip((page - 1) * pageSize)
//                 .Take(pageSize)
//                 .Select(u => new ChatService.UserDto
//                 {
//                     Id = u.Id,
//                     Email = u.Email,
//                     UserName = u.UserName,
//                     EmailConfirmed = u.IsEmailConfirmed
//                 })
//                 .ToListAsync();
//
//             return users;
//         }
//
//         public async Task<ChatService.UserDto> UpdateUserAsync(Guid userId, ChatService.UpdateUserDto updateUserDto)
//         {
//             var user = await _context.Users.FindAsync(userId);
//             if (user == null) return null;
//
//             // Обновляем только если значение не пустое и отличается
//             if (!string.IsNullOrEmpty(updateUserDto.UserName) && 
//                 updateUserDto.UserName != user.UserName)
//             {
//                 user.UserName = updateUserDto.UserName;
//             }
//                 
//             if (!string.IsNullOrEmpty(updateUserDto.Email) && 
//                 updateUserDto.Email != user.Email)
//             {
//                 user.Email = updateUserDto.Email;
//                 user.IsEmailConfirmed = false; // При смене email нужно подтверждение
//             }
//
//             await _context.SaveChangesAsync();
//
//             return new ChatService.UserDto
//             {
//                 Id = user.Id,
//                 Email = user.Email,
//                 UserName = user.UserName,
//                 EmailConfirmed = user.IsEmailConfirmed
//             };
//         }
//
//         public async Task<bool> DeleteUserAsync(Guid userId)
//         {
//             var user = await _context.Users.FindAsync(userId);
//             if (user == null) return false;
//
//             // Проверка: нельзя удалить последнего суперадмина
//             var userRoles = await _context.UserRoles
//                 .Include(ur => ur.Role)
//                 .Where(ur => ur.UserId == userId)
//                 .ToListAsync();
//
//             if (userRoles.Any(ur => ur.Role.RoleName == "SuperAdmin"))
//             {
//                 var superAdminRole = await _context.Roles
//                     .FirstOrDefaultAsync(r => r.RoleName == "SuperAdmin");
//                 
//                 var superAdminCount = await _context.UserRoles
//                     .CountAsync(ur => ur.RoleId == superAdminRole.Id);
//                 
//                 if (superAdminCount <= 1)
//                 {
//                     throw new InvalidOperationException("Нельзя удалить последнего суперадмина");
//                 }
//             }
//
//             _context.Users.Remove(user);
//             await _context.SaveChangesAsync();
//             return true;
//         }
//     }
// }


using BookShop.ADMIN.DTOs;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;

        public UserService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.IsEmailConfirmed
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize)
        {
            var users = await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.IsEmailConfirmed
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(updateUserDto.UserName))
            {
                user.UserName = updateUserDto.UserName;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                user.Email = updateUserDto.Email;
                user.IsEmailConfirmed = false;
            }

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.IsEmailConfirmed
            };
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            if (userRoles.Any(ur => ur.Role.RoleName == "SuperAdmin"))
            {
                var superAdminRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "SuperAdmin");

                var superAdminCount = await _context.UserRoles
                    .CountAsync(ur => ur.RoleId == superAdminRole.Id);

                if (superAdminCount <= 1)
                {
                    throw new InvalidOperationException("Нельзя удалить последнего суперадмина");
                }
            }

            // Удаляем связи UserRole
            _context.UserRoles.RemoveRange(userRoles);

            // Удаляем пользователя
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}