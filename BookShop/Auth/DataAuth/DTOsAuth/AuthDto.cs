// DTO для регистрации пользователя
namespace BookShop.Auth.DataAuth.DTOsAuth
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // DTO для логина пользователя
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // DTO для рефреша токена
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }

    // DTO для ответа с токенами
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
