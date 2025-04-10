namespace BookShop.Auth.DTOAuth.Requests
{
public class RegisterRequest
{
    public string UserName { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty; 
    public string ConfirmPassword { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty; 
}
}
