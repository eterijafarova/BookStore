using System.Net;
using System.Net.Mail;
using System.Text;
using AutoMapper;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ModelsAuth;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Auth.ServicesAuth.Classes
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly LibraryContext _context;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly string _webAppUrl;
        private readonly SmtpClient _smtpClient; 

        public AccountService(
            IMapper mapper,
            LibraryContext authContext,
            IConfiguration config,
            ITokenService tokenService)
        {
            _mapper = mapper;
            _context = authContext;
            _config = config;
            _tokenService = tokenService;
            
            _webAppUrl = _config.GetValue<string>("AppSettings:WebAppUrl")
                ?? throw new InvalidOperationException("AppSettings:WebAppUrl is not configured.");
            
            var smtpSection = _config.GetSection("Email:Smtp");
            var smtpHost = smtpSection["Host"] ?? throw new InvalidOperationException("SMTP Host is missing in configuration.");
            var smtpPort = int.Parse(smtpSection["Port"] ?? throw new InvalidOperationException("SMTP Port is missing in configuration."));
            var smtpUser = smtpSection["User"] ?? throw new InvalidOperationException("SMTP User is missing in configuration.");
            var smtpPass = smtpSection["Pass"] ?? throw new InvalidOperationException("SMTP Pass is missing in configuration.");

            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };
        }
        
        public async Task RegisterAsync(RegisterRequest request)
        {
            try
            {
                var user = _mapper.Map<User>(request);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                user.RefreshToken = Guid.NewGuid().ToString();
                user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var appUserRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "AppUser");
                if (appUserRole == null)
                    throw new Exception("Role 'AppUser' not found in the database.");

                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = appUserRole.Id
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during registration: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                throw;
            }
        }
        
        
        public async Task ConfirmEmailAsync(ConfirmRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.username);
            if (user == null)
                throw new Exception("User not found for email confirmation.");

            var filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "email.html");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Email template not found at {filePath}", filePath);

            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            var template = await sr.ReadToEndAsync();

            var token = await _tokenService.CreateEmailTokenAsync(request.username);
            var link = $"{_webAppUrl}/confirm-email?token={Uri.EscapeDataString(token)}";
            
            var htmlBody = template
                .Replace("{username}", request.username)
                .Replace("{link}", link);
            var plainBody =
                $"Hello, {request.username}!\n\n" +
                $"You have received this email because you have registered on our website. Please confirm your email by clicking the button below:\n{link}\n\n" +
                "If you haven't registered for the \"Cheshire Shelf\" - book shop - just ignore this email.";

            var message = new MailMessage
            {
                From = new MailAddress(_config["Email:Smtp:User"] ?? throw new InvalidOperationException(), "BookShop Team"),
                Subject = "Confirm your email",
                IsBodyHtml = true
            };
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html")
            );
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(plainBody, Encoding.UTF8, "text/plain")
            );
            message.Headers.Add("MIME-Version", "1.0");
            message.Headers.Add("X-Priority", "3");
            message.Headers.Add("Content-Transfer-Encoding", "7bit");
            message.To.Add(user.Email);

            try
            {
                await _smtpClient.SendMailAsync(message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex);
                throw;
            }
        }


        public async Task VerifyEmailAsync(string token)
        {
            var name = await _tokenService.GetNameFromToken(token);
            var isValid = await _tokenService.ValidateEmailTokenAsync(token);
            if (!isValid) return;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == name);
            if (user == null)
                throw new Exception("User not found in VerifyEmail.");

            user.IsEmailConfirmed = true;
            await _context.SaveChangesAsync();
        }
        
        public async Task RequestPasswordResetAsync(RequestPasswordResetRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                throw new Exception("User with this email does not exist.");
            
            var token = await _tokenService.CreateResetPasswordTokenAsync(user.UserName);

            var resetLink = $"{_webAppUrl}/reset-password?token={Uri.EscapeDataString(token)}";
            
            var emailSubject = "Password Reset Request";
            var emailBody = $"Please click the link to reset your password: {resetLink}";

            var message = new MailMessage
            {
                From = new MailAddress(_config["Email:Smtp:User"] ?? throw new InvalidOperationException(), "CheshireShelf Team"),
                Subject = emailSubject,
                IsBodyHtml = false
            };
            message.To.Add(user.Email);
            
            message.Body = emailBody;

            try
            {
                await _smtpClient.SendMailAsync(message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                throw;
            }
        }
        
public async Task ResetPasswordAsync(ResetPasswordRequest request)
{
 
    var isValid = await _tokenService.ValidateResetPasswordTokenAsync(request.Token);
    if (!isValid)
        throw new Exception("The password reset link has expired or is invalid.");
    

    var username = await _tokenService.GetUsernameFromResetToken(request.Token);
    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

    if (user == null)
        throw new Exception("User not found.");
    
    var filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "reset-password.html");
    
    if (!File.Exists(filePath))
        throw new FileNotFoundException($"Reset password template not found at {filePath}", filePath);

    await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var sr = new StreamReader(fs, Encoding.UTF8);
    var template = await sr.ReadToEndAsync();

    var token = await _tokenService.CreateResetPasswordTokenAsync(request.Username);
    var link = $"{_webAppUrl}/reset-password?token={Uri.EscapeDataString(token)}";
    
    var htmlBody = template
        .Replace("{username}", request.Username)
        .Replace("{link}", link);

    var plainBody =
        $"Hello, {request.Username}!\n\n" +
        $"You have requested to reset your password. Please click the link below to reset your password:\n{link}\n\n" +
        "If you did not request a password reset, please ignore this email.";

    var message = new MailMessage
    {
        From = new MailAddress(_config["Email:Smtp:User"] ?? throw new InvalidOperationException(), "CheshireShelf Team"),
        Subject = "Password Reset Request",
        IsBodyHtml = true
    };
    message.AlternateViews.Add(
        AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html")
    );
    message.AlternateViews.Add(
        AlternateView.CreateAlternateViewFromString(plainBody, Encoding.UTF8, "text/plain")
    );
    message.Headers.Add("MIME-Version", "1.0");
    message.Headers.Add("X-Priority", "3");
    message.Headers.Add("Content-Transfer-Encoding", "7bit");
    message.To.Add(user.Email);

    try
    {
        await _smtpClient.SendMailAsync(message); 
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending email: " + ex.Message);
        throw;
    }
}

    }
}
