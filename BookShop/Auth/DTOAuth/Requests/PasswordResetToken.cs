namespace BookShop.Auth.DTOAuth.Requests
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string? NewPassword { get; set; } 
        public DateTime ExpiryDate { get; set; }
    }


}