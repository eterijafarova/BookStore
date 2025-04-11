using Microsoft.AspNetCore.Identity;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using System.Security.Claims;
using System.Security.Cryptography;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Auth.ServicesAuth.Interfaces.BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class AuthService : IAuthService
    {
        private readonly LibraryContext _context;
        private readonly ITokenService _tokenService;

        
        public AuthService(LibraryContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // Метод для аутентификации пользователя
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid username or password");
            }

            return user; 
        }
        

        // Метод для входа (логина) пользователя с созданием токенов
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await AuthenticateAsync(request.UserName, request.Password); 

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName)
            };

            var accessToken = await _tokenService.CreateTokenAsync(claims);
            var refreshToken = GenerateRefreshToken(); 

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.Now.AddDays(7);

            await _context.SaveChangesAsync();

            return new LoginResponse(accessToken, refreshToken);
        }

        // Метод для обновления refresh токена
        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.Username);

            if (user == null || user.RefreshToken != request.RefreshToken)
            {
                throw new Exception("Invalid refresh token");
            }

            if (user.RefreshTokenExpiration < DateTime.Now)
            {
                throw new Exception("Refresh token has expired");
            }

            // Генерация нового refresh токена
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiration = DateTime.Now.AddDays(7);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.Username)
            };

            var newAccessToken = await _tokenService.CreateTokenAsync(claims);

            await _context.SaveChangesAsync();

            return new RefreshTokenResponse(newAccessToken, newRefreshToken);
        }

        // Метод для генерации случайного и безопасного refresh токена
        // Метод для генерации refresh токена
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber); 
        }
    }
}
