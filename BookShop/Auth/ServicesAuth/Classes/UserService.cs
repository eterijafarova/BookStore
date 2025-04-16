// using BookShop.Auth.ModelsAuth;
// using BookShop.Auth.ServicesAuth.Interfaces;
// using BookShop.Data.Contexts;
// using Microsoft.EntityFrameworkCore;
//
//
// // Если ваши UserDto и UpdateUserDto находятся в другом пространстве имён, убедитесь, что подключены правильные using.
// using BookShop.ADMIN.DTOs; 
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
//         // Проверка учетных данных
//         public async Task<User> ValidateUserAsync(string email, string password)
//         {
//             var user = await _context.Users
//                                      .Include(u => u.Role)
//                                      .FirstOrDefaultAsync(u => u.Email == email);
//             if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
//             {
//                 return user;
//             }
//             return null;
//         }
//
//         public async Task UpdateAsync(User user)
//         {
//             _context.Users.Update(user);
//             await _context.SaveChangesAsync();
//         }
//
//         // Удаление пользователя по Id
//         public async Task<bool> DeleteUserAsync(Guid userId)
//         {
//             var user = await _context.Users.FindAsync(userId);
//             if (user == null)
//                 return false;
//             
//             _context.Users.Remove(user);
//             await _context.SaveChangesAsync();
//             return true;
//         }
//
//         // Получение пользователя по Id (возвращает DTO)
//         public async Task<UserDto> GetUserAsync(Guid userId)
//         {
//             var user = await _context.Users
//                                      .Include(u => u.Role)
//                                      .FirstOrDefaultAsync(u => u.Id == userId);
//             if (user == null)
//                 return null;
//
//             return new UserDto
//             {
//                 Id = user.Id,
//                 Email = user.Email,
//                 UserName = user.UserName
//                 // Дополнительный маппинг полей можно добавить при необходимости
//             };
//         }
//
//         public async Task<Role> GetRoleByNameAsync(string roleName)
//         {
//             return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
//         }
//         
//         // Получение списка пользователей с пагинацией (возвращает DTO)
//         public async Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize)
//         {
//             var users = await _context.Users
//                                      .OrderBy(u => u.Email)
//                                      .Skip((page - 1) * pageSize)
//                                      .Take(pageSize)
//                                      .ToListAsync();
//
//             return users.Select(user => new UserDto
//             {
//                 Id = user.Id,
//                 Email = user.Email,
//                 UserName = user.UserName
//             });
//         }
//
//         // Обновление пользователя (возвращает DTO)
//         public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
//         {
//             var user = await _context.Users.FindAsync(userId);
//             if (user == null)
//                 return null;
//
//             user.UserName = updateUserDto.UserName;
//             user.Email = updateUserDto.Email;
//             // При необходимости обновите и другие поля
//
//             _context.Users.Update(user);
//             await _context.SaveChangesAsync();
//
//             return new UserDto
//             {
//                 Id = user.Id,
//                 Email = user.Email,
//                 UserName = user.UserName
//             };
//         }
//
//         // Создание нового пользователя
//         public async Task CreateUserAsync(User newUser)
//         {
//             _context.Users.Add(newUser);
//             await _context.SaveChangesAsync();
//         }
//
//         // Получение пользователя по email (возвращает доменную модель User)
//         public async Task<User> GetUserByEmailAsync(string requestEmail)
//         {
//             return await _context.Users
//                                  .Include(u => u.Role)
//                                  .FirstOrDefaultAsync(u => u.Email == requestEmail);
//         }
//
//         // Получение доменной модели User по идентификатору
//         public async Task<User> GetUserDomainAsync(Guid userId)
//         {
//             return await _context.Users
//                                  .Include(u => u.Role)
//                                  .FirstOrDefaultAsync(u => u.Id == userId);
//         }
//
//         // Получение пользователя по refresh-токену (для обновления токенов)
//         public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
//         {
//             return await _context.Users
//                                  .Include(u => u.Role)
//                                  .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
//         }
//     }
// }
