using BookShop.Data.Enums;

namespace BookShop.ADMIN.DTOs.ChatDtos;

public class MessageDto
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public string SenderName { get; set; }

    public string? Text { get; set; }

    public string? ImageUrl { get; set; }

    public MessageType Type { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsMine { get; set; }
}