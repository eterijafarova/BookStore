using System.Net;
using System.Net.Mail;

using BookShop.Auth.ServicesAuth.Interfaces;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["Email:Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Email:Smtp:Port"]);
            var smtpUser = _configuration["Email:Smtp:User"];
            var smtpPass = _configuration["Email:Smtp:Pass"];
            
            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUser);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}