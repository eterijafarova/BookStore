namespace BookShop.Auth.DTOAuth.Requests
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string Username { get; set; }
    }
}