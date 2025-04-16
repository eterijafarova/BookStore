namespace BookShop.Auth.ModelsAuth
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }


}