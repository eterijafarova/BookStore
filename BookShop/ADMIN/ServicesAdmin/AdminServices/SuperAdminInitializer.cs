using System.Security.Cryptography;
using System.Text;
using BookShop.Auth.ModelsAuth;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; // для использования bcrypt

namespace BookShop.ADMIN.ServicesAdmin.AdminServices;

public class SuperAdminInitializer
{
    private readonly LibraryContext _context;
    private readonly ITokenService _tokenService;

    public SuperAdminInitializer(LibraryContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task CreateSuperAdminAsync()
    {
        var superAdminEmail = "somebody@gmail.com";
        
        var superAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Email == superAdminEmail);

        if (superAdmin == null)
        {
            var password = "Kotik123!";  
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);  
            
            var refreshToken = GenerateRefreshToken();  
            var hashedRefreshToken = HashRefreshToken(refreshToken);  
            
            superAdmin = new User
            {
                Email = superAdminEmail,
                UserName = "Boss_123",
                PasswordHash = hashedPassword,  
                RefreshToken = hashedRefreshToken,  
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(30)  
            };
            
            _context.Users.Add(superAdmin);
            await _context.SaveChangesAsync();
            
            var superAdminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "SuperAdmin");

            if (superAdminRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = superAdmin.Id,  
                    RoleId = superAdminRole.Id  
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }
            else
            {
                superAdminRole = new Role
                {
                    RoleName = "SuperAdmin"
                };

                _context.Roles.Add(superAdminRole);
                await _context.SaveChangesAsync();
                
                var userRole = new UserRole
                {
                    UserId = superAdmin.Id,
                    RoleId = superAdminRole.Id
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }
        }
    }
    
    private string GenerateRefreshToken()
    {
        var guid = Guid.NewGuid();  
        return guid.ToString(); 
    }
    
    private string HashRefreshToken(string refreshToken)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));  
            return Convert.ToBase64String(hashBytes);  
        }
    }
}
