namespace BookShop.ADMIN.DTOs.ChatDtos;

public class SendMessageDto
{
    public Guid ChatId { get; set; }

    public string? Text { get; set; }

    public string? ImageUrl { get; set; }
}