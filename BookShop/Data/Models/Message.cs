namespace BookShop.Data.Models;
using BookShop.Auth.ModelsAuth;
using BookShop.Data.Enums;


public class Message
{
    public Guid Id { get; set; }

    public Guid ChatId { get; set; }

    public Chat Chat { get; set; }

    public Guid SenderId { get; set; }

    public User Sender { get; set; }

    public string? Text { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsRead { get; set; }

    public MessageType Type { get; set; }

    public DateTime SentAt { get; set; }
        = DateTime.UtcNow;
}