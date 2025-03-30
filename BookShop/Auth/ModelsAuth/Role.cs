namespace BookShop.Auth.ModelsAuth;

public class Role
{
    public Guid RoleId { get; set; }  
    public string RoleName { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}