using BookShop.Data.Enums;
using BookShop.Data.Models;
using ChatStatus = BookShop.Data.Enums.ChatStatus;

namespace BookShop.ADMIN.DTOs.ChatDtos;

public class ChatDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public Guid? AdminId { get; set; }

    public string? AdminName { get; set; }

    public ChatStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}