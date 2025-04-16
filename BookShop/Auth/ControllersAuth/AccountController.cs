using BookShop.Auth.DataAuth.Validators;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ServicesAuth.Interfaces;
using ControllerFirst.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Auth.ControllersAuth;

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
        {
            return BadRequest(result.Errors);
        }

        await _accountService.RegisterAsync(request);

        return Ok(new Result<string>(true, request.Username, "Successfully registered"));
    }

    [AllowAnonymous]
    [HttpGet("VerifyEmail")]
    public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
    {
        await _accountService.VerifyEmailAsync(token);
        
        return Ok(new Result<string>(true, "Email confirmed", "Email confirmed"));
    }
    [AllowAnonymous]
    [HttpPost("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmRequest request)
    {
        await _accountService.ConfirmEmailAsync(request, HttpContext);

        return Ok(new Result<string>(true, request.username, "Email sent"));
    }

   
}