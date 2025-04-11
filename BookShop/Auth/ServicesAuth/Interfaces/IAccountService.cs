using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;

namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface IAccountService
{
    Task<Result<RegistrationResponse>> RegisterAsync(RegisterRequest? request); 
    Task ConfirmEmailAsync(ConfirmRequest? request);  
    Task VerifyEmailAsync(string token);
    Task RequestPasswordResetAsync(string email);  
    Task ResetPasswordAsync(string token, string? newPassword);  
}