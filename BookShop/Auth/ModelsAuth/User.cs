using BookShop.Data.Models;

namespace BookShop.Auth.ModelsAuth;

public class User
{
    public Guid Id { get; set; }  // Используем Id как первичный ключ
    public string UserName { get; set; } 

    public string PasswordHash { get; set; } 

    public string Email { get; set; } 

    public bool IsEmailConfirmed { get; set; }

    public string RefreshToken { get; set; } // nullable по умолчанию (null)

    public DateTime? RefreshTokenExpiration { get; set; } = DateTime.Now.AddDays(7);

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}