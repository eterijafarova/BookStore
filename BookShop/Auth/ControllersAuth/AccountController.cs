using BookShop.Auth.DataAuth.Validators;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ServicesAuth.Interfaces;
using ControllerFirst.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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
        

        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest request)
        {
            await _accountService.ConfirmEmailAsync(request);
            return Ok(new Result<string>(true, request.username, "Email sent"));
        }
    }
}
