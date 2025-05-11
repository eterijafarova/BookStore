using BookShop.Auth.DataAuth.Validators;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ServicesAuth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using BookShop.Auth.DTOAuth.Responses;

namespace BookShop.Auth.ControllersAuth
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        
        /// <summary>
        /// API endpoint returning JSON
        /// GET /api/v1/Account/reset-password?token={token}
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _accountService.ResetPasswordAsync(request);
                return Ok(new { message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
   
        [AllowAnonymous]
        [HttpPost("request-password")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
        {
            try
            {
                
                await _accountService.RequestPasswordResetAsync(request.Email);
                return Ok(new { message = "Password reset link sent to your email." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var validator = new RegisterValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            await _accountService.RegisterAsync(request);
            return Ok(new Result<string>(true, request.Username, "Successfully registered"));
        }

        /// <summary>
        /// API endpoint returning JSON
        /// GET /api/v1/Account/VerifyEmail?token={token}
        /// </summary>
        [AllowAnonymous]
        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
        {
            await _accountService.VerifyEmailAsync(token);
            return Ok(new Result<string>(true, "Email confirmed", "Email confirmed"));
        }

        /// <summary>
        /// Web page endpoint redirecting to static HTML pages
        /// GET /confirm-email/{token}
        /// </summary>
        [AllowAnonymous]
        [HttpGet("/confirm-email")]
        public async Task<IActionResult> ConfirmEmailPage([FromQuery] string token)
        {
            try
            {
                await _accountService.VerifyEmailAsync(token);
                return Redirect("/confirm-success.html");
            }
            catch
            {
                return Redirect("/confirm-error.html");
            }
        }
        
        
        /// <summary>
        /// Web page endpoint for resetting password
        /// GET /reset-password-page?token={token}
        /// </summary>
        [AllowAnonymous]
        [HttpGet("/reset-password")]
        public async Task<IActionResult> ResetPasswordPage([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required." });
            }
            var isValidToken = await _accountService.ValidatePasswordResetTokenAsync(token);
            if (!isValidToken)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reset-password.html"), "text/html");
        
        }

        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest request)
        {
            await _accountService.ConfirmEmailAsync(request);
            return Ok(new Result<string>(true, request.username, "Email sent"));
        }
    }
}