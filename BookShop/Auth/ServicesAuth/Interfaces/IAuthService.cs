using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;

namespace BookShop.Auth.ServicesAuth.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<User> AuthenticateAsync(string requestUsername, string requestPassword); // Возвращаем User
    }
}