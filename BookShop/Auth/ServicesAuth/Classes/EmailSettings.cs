namespace BookShop.Auth.ServicesAuth.Classes
{
    public class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public string? SmtpPort { get; set; }
        public string? EmailAddress { get; set; }
        public required string EmailPassword { get; set; }
    }

}
