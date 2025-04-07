using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookShop.Auth.ControllersAuth
{
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

        // Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Set cookies securely
                SetTokensInCookies(response.AccessToken, response.RefreshToken);

                return Ok(new Result<LoginResponse>(true, response, "Successfully logged in"));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occurred during login" });
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
                var request = new RefreshTokenRequest(await _tokenService.GetNameFromToken(accessToken), refreshToken);
                var newTokens = await _authService.RefreshTokenAsync(request);

                SetTokensInCookies(newTokens.AccessToken, newTokens.RefreshToken);

                return Ok(new Result<RefreshTokenResponse>(true, newTokens, "Successfully refreshed token"));
            }
            catch (Exception)
            {
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
        private void SetTokensInCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}
