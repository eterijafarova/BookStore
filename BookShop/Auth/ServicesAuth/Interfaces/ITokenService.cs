namespace BookShop.Auth.ServicesAuth.Interfaces;

public interface ITokenService
{
    Task<string> GetNameFromToken(string token);
    Task<string> CreateTokenAsync(string username);
    Task<string> CreateEmailTokenAsync(string username);
    
    Task<bool> ValidateEmailTokenAsync(string token);

    Task<string> CreatePasswordResetTokenAsync(string username);  
    Task<bool> ValidatePasswordResetTokenAsync(string token);     
        
 
    // Task InvalidateResetPasswordToken(string token); 
   
    
    
}
