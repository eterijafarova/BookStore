using BookShop.ADMIN.DTOs;
using BookShop.Auth.ModelsAuth;


namespace BookShop.Auth.ServicesAuth.Interfaces
{
    public interface IUserService
    {
        // Проверка учетных данных
        Task<User> ValidateUserAsync(string email, string password);
        Task UpdateAsync(User user);

        // Методы работы с пользователями по Guid
        Task<bool> DeleteUserAsync(Guid userId);
        Task<UserDto> GetUserAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize);
        Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task CreateUserAsync(User newUser);
        Task<User> GetUserByEmailAsync(string requestEmail);

        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task<Role> GetRoleByNameAsync(string roleName);

        // Добавляем метод для получения доменной модели User
        Task<User> GetUserDomainAsync(Guid userId);
    }
}