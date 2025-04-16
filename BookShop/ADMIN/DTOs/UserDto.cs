namespace BookShop.ADMIN.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public string EmailConfirmationToken { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime? EmailConfirmationTokenExpiry { get; set; }
        // Можно добавить и другие свойства, которые хотите возвращать
    }

    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        // При необходимости добавьте другие изменяемые поля
    }
}