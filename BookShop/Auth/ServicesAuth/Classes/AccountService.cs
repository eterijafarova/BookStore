using System.Diagnostics;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ServicesAuth.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces.BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class AccountService : IAccountService
    {
        private readonly LibraryContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private IAccountService _accountServiceImplementation;

        public AccountService(LibraryContext context, ITokenService tokenService, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
            _emailService = emailService;
        }

        // Метод для регистрации пользователя
public async Task<Result<RegistrationResponse>> RegisterAsync(RegisterRequest? request)
{
    try
    {
        // Проверка на наличие пользователя с таким же именем или email
        if (await _context.Users.AnyAsync(u => request != null && u.UserName == request.UserName))
        {
            return Result<RegistrationResponse>.Error(null, "Username already exists");
        }

        if (await _context.Users.AnyAsync(u => request != null && u.Email == request.Email))
        {
            return Result<RegistrationResponse>.Error(null, "Email already exists");
        }

        // Генерация нового Refresh Token
        var refreshToken = Guid.NewGuid().ToString();

        // Создание нового пользователя
        Debug.Assert(request != null, nameof(request) + " != null");
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),  // Хэшируем пароль с использованием SHA-256
            RefreshToken = refreshToken, // Сохраняем Refresh Token
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7) // Устанавливаем срок действия Refresh Token
        };

        // Сохранение нового пользователя в базу данных
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Создание списка заявок (claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "User") // Можно добавить другие claims, если нужно
        };

        // Генерация JWT Access Token
        var accessToken = await _tokenService.CreateTokenAsync(claims);

        // Возвращаем оба токена в ответе
        var response = new RegistrationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result<RegistrationResponse>.Success(response, "Successfully registered");
    }
    catch (Exception ex)
    {
        // Логируем ошибку для подробного анализа
        Console.WriteLine($"Error during registration: {ex.Message}");
        return Result<RegistrationResponse>.Error(null, $"Error during registration: {ex.Message}");
    }
}



        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedBytes);  
            }
        }

        
        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            
            var token = Guid.NewGuid().ToString();
            
            var resetLink = $"{_config["AppUrl"]}/reset-password?token={token}";
            await _emailService.SendEmailAsync(user.Email, "Password Reset Request", 
                $"Please click the following link to reset your password: {resetLink}");
            
            user.RefreshToken = token;
            user.RefreshTokenExpiration = DateTime.Now.AddHours(1); 
            await _context.SaveChangesAsync();
        }
        
        public async Task ResetPasswordAsync(string token, string? newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
            if (user == null || user.RefreshTokenExpiration < DateTime.Now)
            {
                throw new Exception("Invalid or expired token");
            }

            var passwordHasher = new PasswordHasher<User>();
            Debug.Assert(newPassword != null, nameof(newPassword) + " != null");
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

     
            user.RefreshToken = null;
            user.RefreshTokenExpiration = DateTime.Now.AddMinutes(-1); 
            await _context.SaveChangesAsync();

        }

        // Метод для подтверждения email
        public async Task ConfirmEmailAsync(ConfirmRequest? request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => request != null && u.UserName == request.UserName);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var token = Guid.NewGuid().ToString();
            
            user.RefreshToken = token;
            user.RefreshTokenExpiration = DateTime.Now.AddHours(1);  
            await _context.SaveChangesAsync();
            
            var confirmationLink = $"{_config["AppUrl"]}/confirm-email?token={token}";
            
            await _emailService.SendEmailAsync(user.Email, "Confirm your email", 
                $"Please click the following link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>");
        }

        // Метод для верификации email по токену
        public async Task VerifyEmailAsync(string token)
        {
            var username = await _tokenService.GetNameFromToken(token);
            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("Invalid token");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Подтверждаем email пользователя
            user.IsEmailConfirmed = true;
            user.RefreshToken = null;  
            user.RefreshTokenExpiration = DateTime.Now.AddMinutes(-1); 
          

            await _context.SaveChangesAsync();
        }
    }
}
