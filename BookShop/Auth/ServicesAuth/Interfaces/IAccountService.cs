using BookShop.Auth.DTOAuth.Requests;

namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest request);
    Task ConfirmEmailAsync(ConfirmRequest request);
    Task VerifyEmailAsync(string token);

    // Новый метод для сброса пароля
    Task ResetPasswordAsync(ResetPasswordRequest request); // Добавлен метод для сброса пароля
    Task RequestPasswordResetAsync(RequestPasswordResetRequest request);
}