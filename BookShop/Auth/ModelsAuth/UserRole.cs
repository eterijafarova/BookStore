namespace BookShop.Auth.ModelsAuth
{
    public class UserRole
    {
        public Guid UserRoleId { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; }
        
        public Guid RoleId { get; set; }
        
        public Role Role { get; set; }
        public User User { get; set; }
    }
}