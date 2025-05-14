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

        public AccountService(
            IMapper mapper,
            LibraryContext authContext,
            IConfiguration config,
            ITokenService tokenService)
        {
            _mapper       = mapper;
            _context      = authContext;
            _config       = config;
            _tokenService = tokenService;
            
            _webAppUrl = _config.GetValue<string>("AppSettings:WebAppUrl")
                ?? throw new InvalidOperationException("AppSettings:WebAppUrl is not configured.");
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
        
        
    public async Task RequestPasswordResetAsync(string email)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null)
        throw new Exception("User not found.");

    var token = await _tokenService.CreatePasswordResetTokenAsync(user.UserName);
    var resetLink = $"{_webAppUrl}/reset-password?token={Uri.EscapeDataString(token)}";

    var smtpSection = _config.GetSection("Email:Smtp");
    var smtpHost = smtpSection["Host"]
        ?? throw new InvalidOperationException("SMTP Host is missing in configuration.");
    var smtpPort = int.Parse(smtpSection["Port"]
        ?? throw new InvalidOperationException("SMTP Port is missing in configuration."));
    var smtpUser = smtpSection["User"]
        ?? throw new InvalidOperationException("SMTP User is missing in configuration.");
    var smtpPass = smtpSection["Pass"]
        ?? throw new InvalidOperationException("SMTP Pass is missing in configuration.");

    using var client = new SmtpClient(smtpHost, smtpPort)
    {
        EnableSsl = true,
        Credentials = new NetworkCredential(smtpUser, smtpPass)
    };

    var baseDir = AppContext.BaseDirectory;
    var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
    var filePath = Path.Combine(projectRoot, "wwwroot", "email.html");

    if (!File.Exists(filePath))
        throw new FileNotFoundException($"Email template not found at {filePath}", filePath);

    await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var sr = new StreamReader(fs, Encoding.UTF8);
    var template = await sr.ReadToEndAsync();
    
    var htmlBody = template
        .Replace("{username}", user.UserName) 
        .Replace("{reset-link}", resetLink); 

    var plainBody = $"Hello, {user.UserName}!\n\n" +
                    $"You have received this email because you have requested a password reset. Please click the following link to reset your password:\n{resetLink}\n\n" +
                    "If you didn't request a password reset, please ignore this email.";

    var message = new MailMessage
    {
        From = new MailAddress(smtpUser, "CheshireSchelf Team"),
        Subject = "Reset Password",
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
        await client.SendMailAsync(message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending email: " + ex.Message);
        throw;
    }


    var passwordResetToken = new PasswordResetToken
    {
        Token = token,
        UserName = user.UserName,
        ExpiryDate = DateTime.UtcNow.AddHours(1),
        NewPassword = "" 
    };

    await _context.PasswordResetTokens.AddAsync(passwordResetToken);
    await _context.SaveChangesAsync();
}


public async Task<bool> ValidatePasswordResetTokenAsync(string token)
{
    var passwordResetToken = await _context.PasswordResetTokens
        .FirstOrDefaultAsync(t => t.Token == token);

    if (passwordResetToken == null)
    {
        return false; 
    }
    
    if (passwordResetToken.ExpiryDate < DateTime.UtcNow)
    {
        return false; 
    }

    return true; 
}
        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var isValidToken = await _tokenService.ValidatePasswordResetTokenAsync(request.Token);
            if (!isValidToken)
                throw new Exception("Invalid or expired password reset token.");

            var userName = await _tokenService.GetNameFromToken(request.Token);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                throw new Exception("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
        }


        
        public async Task ConfirmEmailAsync(ConfirmRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.username);
            if (user == null)
                throw new Exception("User not found for email confirmation.");

            var smtpSection = _config.GetSection("Email:Smtp");
            var smtpHost    = smtpSection["Host"]
                ?? throw new InvalidOperationException("SMTP Host is missing in configuration.");
            var smtpPort    = int.Parse(smtpSection["Port"]
                ?? throw new InvalidOperationException("SMTP Port is missing in configuration."));
            var smtpUser    = smtpSection["User"]
                ?? throw new InvalidOperationException("SMTP User is missing in configuration.");
            var smtpPass    = smtpSection["Pass"]
                ?? throw new InvalidOperationException("SMTP Pass is missing in configuration.");

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl   = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var baseDir     = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var filePath    = Path.Combine(projectRoot, "wwwroot", "email.html");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Email template not found at {filePath}", filePath);

            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr       = new StreamReader(fs, Encoding.UTF8);
            var template       = await sr.ReadToEndAsync();

            var token = await _tokenService.CreateEmailTokenAsync(request.username);
            var link  = $"{_webAppUrl}/confirm-email?token={Uri.EscapeDataString(token)}";
            
            var htmlBody = template
                .Replace("{username}", request.username)
                .Replace("{link}", link);
            var plainBody =
                $"Hello, {request.username}!\n\n" +
                $"You have received this email because you have registered on our website. Please confirm your email by clicking the button below:\n{link}\n\n" +
                "If you haven't registered for the \"Cheshire Shelf\" - book shop - just ignore this email.";

            var message = new MailMessage
            {
                From       = new MailAddress(smtpUser, "CheshireShelf Team"),
                Subject    = "Confirm your email",
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
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex);
                throw;
            }
        }

        public async Task VerifyEmailAsync(string token)
        {
            var name    = await _tokenService.GetNameFromToken(token);
            var isValid = await _tokenService.ValidateEmailTokenAsync(token);
            if (!isValid) return;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == name);
            if (user == null)
                throw new Exception("User not found in VerifyEmail.");

            user.IsEmailConfirmed = true;
            await _context.SaveChangesAsync();
        }
        
    }
}