using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BookShop.Auth.ServicesAuth.Interfaces.BookShop.Auth.ServicesAuth.Interfaces;

namespace BookShop.Auth.ControllersAuth
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        // Регистрация пользователя
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest? request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new Result<string>(false, null, "Request data is invalid"));
                }

                var result = await _accountService.RegisterAsync(request); // Регистрация

                if (!result.IsSuccess)
                {
                    return BadRequest(new Result<string>(false, null, result.Message));
                }

                Debug.Assert(result.Data != null, "result.Data != null");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.UserName)
                };

                var accessToken = await _tokenService.CreateTokenAsync(claims, expirationMinutes: 15);
                var refreshToken = await _tokenService.CreateTokenAsync(claims, expirationMinutes: 60);

                // Вернем оба токена
                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during registration: {ex.Message}"));
            }
        }

        // Подтверждение email
        [Authorize]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest? request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Username))
                {
                    return BadRequest(new Result<string>(false, null, "Invalid email confirmation request"));
                }

                await _accountService.ConfirmEmailAsync(request);

                return Ok(Result<string>.Success(request.Username, "Email confirmation link sent"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during email confirmation: {ex.Message}"));
            }
        }

        // Запрос на сброс пароля
        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new Result<string>(false, null, "Email is required for password reset"));
                }

                await _accountService.RequestPasswordResetAsync(email);

                return Ok(new Result<string>(true, null, "Password reset link sent to your email"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during password reset request: {ex.Message}"));
            }
        }

        // Сброс пароля
        [HttpPost("ResetPassword/{token}")] // Используем параметр {token}
        public async Task<IActionResult> ResetPassword([FromRoute] string token, [FromBody] ResetPasswordRequest? request)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || request == null || string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new Result<string>(false, null, "Invalid token or password"));
                }

                await _accountService.ResetPasswordAsync(token, request.NewPassword);

                return Ok(new Result<string>(true, null, "Password successfully reset"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during password reset: {ex.Message}"));
            }
        }
    }
}
