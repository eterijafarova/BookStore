using System.Security.Claims;
using System.Security.Cryptography;
using BookShop.ADMIN.DTOs;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Auth.ServicesAuth.Interfaces.BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookShop.Auth.ControllersAuth
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly LibraryContext _context;
        private readonly ILogger<AuthController> _logger;  // Логгер

        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            LibraryContext context,
            ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
            // Логин
            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginRequest dto)
            {
                try
                {
                    var (accessToken, refreshToken) = await _authService.LoginAsync(dto);
                    return Ok(new { accessToken, refreshToken });
                }
                catch (Exception ex)
                {
                    return Unauthorized($"Invalid username or password: {ex.Message}");
                }
            }

            


        // Refresh Token
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var accessToken = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "Invalid tokens" });
            }

            try
            {
                var request = new RefreshTokenRequest(await _tokenService.GetNameFromToken(accessToken!), refreshToken!); // Assert that tokens are not null
                var newTokens = await _authService.RefreshTokenAsync(request);

                SetTokensInCookies(newTokens.AccessToken!, newTokens.RefreshToken!); // Assert that tokens are not null

                return Ok(new Result<RefreshTokenResponse>(true, newTokens, "Successfully refreshed token"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return Unauthorized(new { message = "Invalid refresh token" });
            }
        }

        // Logout
        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Successfully logged out" });
        }

        // Test
        [HttpPost("Test")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Test()
        {
            return Ok("Test successful");
        }

        // Helper method to set tokens in cookies
        private void SetTokensInCookies(string accessToken, string? refreshToken)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException("Tokens cannot be null or empty");
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,  // Устанавливается только если используется HTTPS
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("accessToken", accessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        // Метод для генерации refresh токена
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber); // Генерируем refresh token в виде Base64 строки
        }
    }
}


// using BookShop.ADMIN.DTOs.DTOAdmin;
// using BookShop.ADMIN.ServicesAdmin.AdminServices;
//
// namespace BookShop.ADMIN.ControllersAdmin;
// using Microsoft.AspNetCore.Mvc;
//
// [ApiController]
// [Route("api/[controller]")]
// public class AdminController : ControllerBase
// {
//     private readonly IAdminService _adminService;
//
//     public AdminController(IAdminService adminService)
//     {
//         _adminService = adminService;
//     }
//
//     [HttpPost("login")]
//     public async Task<IActionResult> Login([FromBody] AdminLoginDto dto)
//     {
//         try
//         {
//             var token = await _adminService.LoginAsync(dto);
//             return Ok(new { token });
//         }
//         catch
//         {
//             return Unauthorized("Invalid username or password");
//         }
//     }
//
//     [HttpPost("register")]
//     public async Task<IActionResult> Register([FromBody] AdminRegisterDto dto)
//     {
//         var result = await _adminService.RegisterAsync(dto);
//         if (!result)
//             return BadRequest("This admin already exists");
//         return Ok("Admin created");
//     }
// }
