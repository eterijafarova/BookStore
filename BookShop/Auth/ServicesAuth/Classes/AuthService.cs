using AutoMapper;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public AuthService(LibraryContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Найти пользователя по имени
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Создаем access-токен через TokenService
            var accessToken = await _tokenService.CreateTokenAsync(request.username);

            // Генерируем refresh-токен как строку (GUID)
            var refreshToken = Guid.NewGuid().ToString();
            // Используем UtcNow для единообразия
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);
            
            // Обновляем свойства пользователя
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = refreshTokenExpiration;
            
            await _context.SaveChangesAsync();
            
            return new LoginResponse(accessToken, refreshToken);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Сравниваем текущий refresh-токен с переданным и проверяем срок действия
            bool oldRefreshTokenValid = user.RefreshToken == request.refreshToken 
                                          && user.RefreshTokenExpiration > DateTime.UtcNow;
            if (!oldRefreshTokenValid)
            {
                throw new Exception("Invalid refresh token");
            }

            // Генерируем новый refresh-токен
            var newRefreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            // Генерируем новый access-токен
            var newAccessToken = await _tokenService.CreateTokenAsync(request.username);

            return new RefreshTokenResponse(newAccessToken, newRefreshToken);
        }
    }
}
