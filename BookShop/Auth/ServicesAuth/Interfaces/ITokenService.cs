namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface ITokenService
{
    Task<string> GetNameFromToken(string token);
    Task<string> CreateTokenAsync(string username);
    Task<string> CreateEmailTokenAsync(string username);
    
    Task<bool> ValidateEmailTokenAsync(string token);
    
    // Новый метод для создания токена для сброса пароля
    Task<string> CreateResetPasswordTokenAsync(string username);
    
    // Новый метод для валидации токена для сброса пароля
    Task<bool> ValidateResetPasswordTokenAsync(string token);
    
    Task<string> GetUsernameFromResetToken(string requestToken);
}
