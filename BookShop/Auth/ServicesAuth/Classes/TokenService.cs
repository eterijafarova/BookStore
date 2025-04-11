using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using System.Threading.Tasks;
using BookShop.Auth.ServicesAuth.Interfaces.BookShop.Auth.ServicesAuth.Interfaces;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly LibraryContext _context;

        public TokenService(IConfiguration config, LibraryContext context)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Метод для создания токена с возможностью настройки времени жизни
        public async Task<string> CreateTokenAsync(List<Claim> claims, int expirationMinutes = 15)
        {
            var jwtKey = _config["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT:Key is not configured");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                signingCredentials: signingCred
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(securityToken));
        }

        // Метод для получения имени пользователя из токена
        public async Task<string> GetNameFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _config["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
                
                var username = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    throw new SecurityTokenException("Username not found in token");
                }

                return await Task.FromResult(username);
            }
            catch (SecurityTokenException ex)
            {
                throw new SecurityTokenException($"Token validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Общая обработка ошибок
                throw new Exception($"An error occurred while validating the token: {ex.Message}");
            }
        }
        
        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            // Логика для обновления access token с использованием refreshToken
            // Например, проверить refreshToken, получить новые claims, создать новый токен
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "user") // Пример, обновите в зависимости от вашего случая
            };

            // Здесь добавьте логику для создания нового access токена
            return await CreateTokenAsync(claims, expirationMinutes: 15);
        }
    }
}
