using BookShop.Auth.DTOAuth.Requests;
using System.Threading.Tasks;

namespace BookShop.Auth.ServicesAuth.Interfaces
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterRequest request);
        
        Task ConfirmEmailAsync(ConfirmRequest request);
        
        Task VerifyEmailAsync(string token);
        
        Task RequestPasswordResetAsync(string email);
        
        Task ResetPasswordAsync(ResetPasswordRequest request);
        Task<bool> ValidatePasswordResetTokenAsync(string token);
    }
}

