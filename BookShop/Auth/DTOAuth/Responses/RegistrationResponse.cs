namespace BookShop.Auth.DTOAuth.Responses;

public class RegistrationResponse
{
    public string Message { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
