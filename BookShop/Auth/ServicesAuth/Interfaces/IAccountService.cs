using BookShop.Auth.DTOAuth.Requests;

namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface IAccountService
{
    public Task RegisterAsync(RegisterRequest request);
    public Task ConfirmEmailAsync(ConfirmRequest request);
    public Task VerifyEmailAsync(string token);
    // public Task ResetPasswordAsync(ResetPasswordRequest request);
}