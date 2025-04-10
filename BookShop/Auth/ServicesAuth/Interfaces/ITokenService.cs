using System.Security.Claims;
using System.Threading.Tasks;

namespace BookShop.Auth.ServicesAuth.Interfaces
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace BookShop.Auth.ServicesAuth.Interfaces
    {
        public interface ITokenService
        {
            Task<string> CreateTokenAsync(List<Claim> claims, int expirationMinutes = 15);
            Task<string> RefreshAccessTokenAsync(string refreshToken); 
            Task<string> GetNameFromToken(string token);
        }
    }

}