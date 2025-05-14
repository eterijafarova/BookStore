namespace BookShop.Auth.DTOAuth.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; } 
        public Guid UserId { get; set; }

        public LoginResponse(string accessToken, string refreshToken, string role, Guid userId)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Role = role;
            UserId = userId;
        }
    }
}