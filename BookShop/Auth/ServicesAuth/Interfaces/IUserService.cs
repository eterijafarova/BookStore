using BookShop.ADMIN.DTOs;
using BookShop.Auth.ModelsAuth;


namespace BookShop.Auth.ServicesAuth.Interfaces
{
    public interface IUserService
    {
        Task<bool> DeleteUserAsync(Guid userId);
        Task<UserDto> GetUserAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize);
        Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);

    }
}