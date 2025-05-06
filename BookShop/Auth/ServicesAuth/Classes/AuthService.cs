using AutoMapper;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;

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
            var user = await _context.Users
                .Where(x => x.UserName == request.username)
                .Include(u => u.UserRoles) // Загружаем связи с ролями
                .ThenInclude(ur => ur.Role) // Загружаем саму роль
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Предполагаем, что у пользователя только одна роль
            var role = user.UserRoles.FirstOrDefault()?.Role?.RoleName;

            if (role == null)
            {
                throw new Exception("User role not found");
            }

            var accessToken = await _tokenService.CreateTokenAsync(request.username);
            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = refreshTokenExpiration;

            await _context.SaveChangesAsync();

            // Возвращаем токены и роль
            return new LoginResponse(accessToken, refreshToken, role);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == request.username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            bool oldRefreshTokenValid = user.RefreshToken == request.refreshToken
                                          && user.RefreshTokenExpiration > DateTime.UtcNow;
            if (!oldRefreshTokenValid)
            {
                throw new Exception("Invalid refresh token");
            }

            var newRefreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            var newAccessToken = await _tokenService.CreateTokenAsync(request.username);

            return new RefreshTokenResponse(newAccessToken, newRefreshToken);
        }
    }
}
