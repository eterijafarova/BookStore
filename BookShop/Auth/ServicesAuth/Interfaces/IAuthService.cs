using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;

namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface IAuthService
{
    
    public Task<LoginResponse> LoginAsync(LoginRequest request);
    
    public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    
}