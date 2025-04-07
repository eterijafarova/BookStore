using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BookShop.Auth.ControllersAuth
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Регистрация пользователя
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _accountService.RegisterAsync(request);

                // Проверка результата
                if (!result.IsSuccess)
                {
                    return BadRequest(new Result<string>(false, null, result.Message));  // Возвращаем ошибку с сообщением
                }

                return Ok(Result<string>.Success(result.Data, "Successfully registered"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during registration: {ex.Message}"));
            }
        }

        // Подтверждение email по токену
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string token)
        {
            try
            {
                await _accountService.VerifyEmailAsync(token);
                return Ok(new Result<string>(true, null, "Email successfully confirmed"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during email confirmation: {ex.Message}"));
            }
        }

        // Отправка ссылки для подтверждения email
        [Authorize] // Защищаем метод
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest request)
        {
            try
            {
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
                await _accountService.RequestPasswordResetAsync(email);
                return Ok(new Result<string>(true, null, "Password reset link sent to your email"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during password reset request: {ex.Message}"));
            }
        }

        // Сброс пароля
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _accountService.ResetPasswordAsync(request.Token, request.NewPassword);
                return Ok(new Result<string>(true, null, "Password successfully reset"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Result<string>(false, null, $"Error during password reset: {ex.Message}"));
            }
        }

        // Сброс пароля (не реализован)
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync()
        {
            // Вернуть ошибку с четким сообщением, что функциональность не реализована
            return StatusCode(501, new Result<string>(false, null, "Reset password functionality is not implemented yet"));
        }
    }
}
