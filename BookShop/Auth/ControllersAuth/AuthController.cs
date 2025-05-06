using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using ControllerFirst.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Auth.ControllersAuth;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        
        Response.Cookies.Append("accessToken", response.AccessToken);
        Response.Cookies.Append("refreshToken", response.RefreshToken);
        
        return Ok(new Result<LoginResponse>(true, response, "Successfully logged in"));
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var accessToken = Request.Cookies["accessToken"];

        var request = new RefreshTokenRequest(await _tokenService.GetNameFromToken(accessToken), refreshToken);

        var newTokens = await _authService.RefreshTokenAsync(request);
        
        Response.Cookies.Append("accessToken", newTokens.accessToken);
        Response.Cookies.Append("refreshToken", newTokens.refreshToken);

        return Ok(new Result<RefreshTokenResponse>(true, newTokens, "Successfully refreshed token"));
    }
    
    [HttpPost("Test")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Test()
    {
        return Ok("Test");
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok("Logout");
    }
}
