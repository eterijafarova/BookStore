namespace BookShop.ADMIN.DTOs
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    
    // DTO для логина пользователя
    public class UserLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

// DTO для регистрации пользователя
    public class UserRegisterDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

}