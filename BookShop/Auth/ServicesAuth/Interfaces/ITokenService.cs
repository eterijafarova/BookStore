namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface ITokenService
{
    Task<string> GetNameFromToken(string token);
    Task<string> CreateTokenAsync(string username);
    Task<string> CreateEmailTokenAsync(string username);
    
    Task<bool> ValidateEmailTokenAsync(string token);

    Task<string> CreateResetPasswordTokenAsync(string username);

    Task<bool> ValidateResetPasswordTokenAsync(string token);
    
    Task<string> GetUsernameFromResetToken(string requestToken);
   
    
}
