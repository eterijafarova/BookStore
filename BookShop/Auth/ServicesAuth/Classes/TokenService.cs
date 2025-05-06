using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly LibraryContext _context;

        public TokenService(IConfiguration config, LibraryContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task<string> GetNameFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (securityToken == null)
                    throw new SecurityTokenException("Invalid token");

                var usernameClaim = securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                if (usernameClaim == null)
                    throw new Exception("Username claim is missing in token.");

                return usernameClaim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting name from token: {ex.Message}");
                throw;
            }
        }

        public async Task<string> CreateTokenAsync(string username)
        {
            // Находим пользователя по имени
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                throw new Exception("User not found.");

            // Загружаем роли пользователя через связь UserRoles
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .AsNoTracking()
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            foreach (var ur in userRoles)
            {
                // Добавляем claim для каждой роли
                claims.Add(new Claim(ClaimTypes.Role, ur.Role.RoleName));
            }

            // Получаем секретный ключ из конфигурации (убедитесь, что он указан в appsettings.json как "JWT:SecretKey")
            var secretKeyValue = _config["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKeyValue))
            {
                throw new Exception("JWT SecretKey is missing in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyValue));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Создаем токен с указанными параметрами
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JWT:AccessTokenExpiryMinutes"])),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<string> CreateEmailTokenAsync(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            
            var emailKey = _config["JWT:EmailKey"];
            if (string.IsNullOrEmpty(emailKey))
            {
                throw new Exception("JWT EmailKey is missing in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(emailKey));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(3),
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<bool> ValidateEmailTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var emailKey = _config["JWT:EmailKey"];
            if (string.IsNullOrEmpty(emailKey))
            {
                throw new Exception("JWT EmailKey is missing in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(emailKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["JWT:Issuer"],
                ValidAudience = _config["JWT:Audience"],
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal.Identity != null && principal.Identity.IsAuthenticated;
            }
            catch
            {
                return false;
            }
        }
        
        
        //
        public async Task<string> CreatePasswordResetTokenAsync(string userName)
        {
            var token = Guid.NewGuid().ToString(); // Генерация уникального токена
            var expirationDate = DateTime.UtcNow.AddHours(1);  // Токен действует 1 час

            // Сохраняем токен в базе данных
            var resetToken = new PasswordResetToken
            {
                Token = token,
                UserName = userName,
                ExpiryDate = expirationDate
            };

            await _context.PasswordResetTokens.AddAsync(resetToken);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string token)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            if (resetToken == null)
                return false;

            if (resetToken.ExpiryDate < DateTime.UtcNow)
                return false;  

            return true;
        }

        public async Task<string> GetUsernameFromResetToken(string token)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            return resetToken?.UserName;
        }

        public async Task InvalidateResetPasswordToken(string token)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            if (resetToken != null)
            {
                _context.PasswordResetTokens.Remove(resetToken); // Удаляем использованный токен
                await _context.SaveChangesAsync();
            }
        }


    }
}