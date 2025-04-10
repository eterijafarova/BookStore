namespace BookShop.Auth.DTOAuth.Responses;

public class RegistrationResponse
{
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

}