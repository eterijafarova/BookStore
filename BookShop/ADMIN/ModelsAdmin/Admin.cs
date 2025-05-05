using BookShop.Auth.ModelsAuth;

namespace BookShop.ADMIN.ModelsAdmin;

// Models/Admin.cs
public class Admin
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Guid UserId { get; set; }  // Внешний ключ для связи с таблицей User
    public User User { get; set; }  // Связь с пользователем
       
}


