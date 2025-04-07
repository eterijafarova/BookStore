using BookShop.ADMIN.DTOs;
using BookShop.Auth.DTOAuth.Responses;

namespace BookShop.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();  // Получить список всех пользователей
        Task<UserDto?> GetByIdAsync(Guid id);  // Получить пользователя по ID
        Task<bool> DeleteAsync(Guid id);  // Удалить пользователя по ID
    }
}