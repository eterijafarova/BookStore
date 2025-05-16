namespace BookShop.ADMIN.DTOs;

public class UsersGetDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public List<string> Roles { get; set; }
}