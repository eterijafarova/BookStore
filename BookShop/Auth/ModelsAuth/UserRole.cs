namespace BookShop.Auth.ModelsAuth
{
    public class UserRole
    {
        // Генерируем новый уникальный идентификатор для связи
        public Guid UserRoleId { get; set; } = Guid.NewGuid();

        // Внешний ключ к пользователю — должен иметь тип Guid, совпадающий с первичным ключом в User
        public Guid UserId { get; set; }

        // Внешний ключ к роли — должен иметь тип Guid, совпадающий с первичным ключом в Role
        public Guid RoleId { get; set; }

        // Навигационные свойства
        public Role Role { get; set; }
        public User User { get; set; }
    }
}