using BookShop.Auth.ModelsAuth;
using BookShop.Data.Models;

public class Chat
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid? AdminId { get; set; }
    public User Admin { get; set; }

    public ChatStatus Status { get; set; } = ChatStatus.Waiting;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}