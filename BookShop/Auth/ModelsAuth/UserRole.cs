namespace BookShop.Auth.ModelsAuth
{
    public class UserRole
    {
        public int UserRoleId { get; set; }  

        public int UserId { get; set; }  
        public int RoleId { get; set; } 

        public Role Role { get; set; }  
        public User User { get; set; }  
    }
}
