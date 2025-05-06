namespace BookShop.Auth.DTOAuth.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; } 

        public LoginResponse(string accessToken, string refreshToken, string role)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Role = role;
        }
    }
}