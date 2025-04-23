using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ServicesAuth.Interfaces;
using ControllerFirst.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShop.Auth.DataAuth.Validators;
using BookShop.Data.Contexts;


namespace BookShop.Auth.ControllersAuth
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;  
        private readonly LibraryContext _context;       

        public AccountController(IAccountService accountService, ITokenService tokenService, LibraryContext context)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var validator = new RegisterValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                string errors = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new Result<string>(false, null, errors));
            }

            try
            {
                await _accountService.RegisterAsync(request);
                return Ok(new Result<string>(true, request.Username, "Successfully registered"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Result<string>(false, null, $"Error during registration: {ex.Message}"));
            }
        }

     
        [AllowAnonymous]
        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new Result<string>(false, null, "Token is required."));

            try
            {
                await _accountService.VerifyEmailAsync(token);
                return Ok(new Result<string>(true, "Email confirmed", "Email confirmed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Result<string>(false, null, $"Error confirming email: {ex.Message}"));
            }
        }
        
        
        [AllowAnonymous]
        [HttpGet("/confirmEmail")]
        public async Task<IActionResult> ConfirmEmailPage([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return Redirect("/confirm-error.html");

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
        
        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.username))
                return BadRequest(new Result<string>(false, null, "Username is required"));

            try
            {
                await _accountService.ConfirmEmailAsync(request);
                return Ok(new Result<string>(true, request.username, "Email sent"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Result<string>(false, null, $"Error during email confirmation: {ex.Message}"));
            }
        }
        
        [HttpPost("requestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                return BadRequest(new Result<string>(false, null, "Email is required"));

            try
            {
                await _accountService.RequestPasswordResetAsync(request);
                return Ok(new Result<string>(true, request.Email, "Password reset link sent to your email."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Result<string>(false, null, $"Error requesting password reset: {ex.Message}"));
            }
        }

        // Сброс пароля
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { message = "Token and new password are required." });
            }

            try
            {
                var isValid = await _tokenService.ValidateResetPasswordTokenAsync(request.Token);
                if (!isValid)
                {
                    return BadRequest(new { message = "The password reset link has expired or is invalid." });
                }
                
                var username = await _tokenService.GetUsernameFromResetToken(request.Token);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

                if (user == null)
                {
                    return BadRequest(new { message = "User not found." });
                }
                
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password reset successful!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error resetting password: {ex.Message}" });
            }
        }


        [AllowAnonymous]
        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPasswordPage([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Invalid or missing token" });
            }
            
            bool isTokenValid = await _tokenService.ValidateResetPasswordTokenAsync(token);
            if (!isTokenValid)
            {
                return BadRequest(new { message = "The password reset link has expired or is invalid." });
            }

            // ссылка на страницу сброса пароля
            string resetPasswordLink = $"http://localhost:3000/reset-password?token={Uri.EscapeDataString(token)}";

            return Ok(new { message = "Valid token, proceed to reset your password.", resetPasswordLink });
        }


    }
}
