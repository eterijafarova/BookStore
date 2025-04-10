using BookShop.ADMIN.DTOs.DTOAdmin;

namespace BookShop.ADMIN.ServicesAdmin.AdminServices;

public interface IAdminService
{
    Task<string> LoginAsync(AdminLoginDto dto);
    Task<bool> RegisterAsync(AdminRegisterDto dto);
}
