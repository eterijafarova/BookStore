using System.Net;
using System.Net.Mail;
using BookShop.Auth.ServicesAuth.Interfaces;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly string _smtpPort;
        private readonly string _emailAddress;
        private readonly string _emailPassword;

        public EmailService(string smtpServer, string smtpPort, string emailAddress, string emailPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _emailAddress = emailAddress;
            _emailPassword = emailPassword;
        }


        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = int.Parse(_smtpPort),
                Credentials = new NetworkCredential(_emailAddress, _emailPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}