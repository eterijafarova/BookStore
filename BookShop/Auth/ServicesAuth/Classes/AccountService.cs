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

        public async Task ConfirmEmailAsync(ConfirmRequest request, HttpContext context)
        {
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.username);
            if (user == null)
                throw new Exception("User not found for email confirmation.");
            
            
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
            var filePath = Path.Combine(projectRoot, "Auth", "wwwroot", "email.html");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Email template not found at {filePath}", filePath);


            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            var template = await sr.ReadToEndAsync();

         
            var token = await _tokenService.CreateEmailTokenAsync(request.username);
            var link = $"{context.Request.Scheme}://{context.Request.Host}/api/v1/Account/VerifyEmail?token={token}";
            var body = template
                .Replace("{username}", request.username)
                .Replace("{link}", link);

            
            var message = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = "Email confirmation",
                Body = body,
                IsBodyHtml = true
            };
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
    }
}