using BookShop.Data.Models;

namespace BookShop.Services.Interfaces;

public interface IChatService
{
    Task<Chat> CreateChatAsync(Guid userId);

    Task<List<Chat>> GetWaitingChatsAsync();

    Task<List<Chat>> GetUserChatsAsync(Guid userId);

    Task<Chat?> GetChatByIdAsync(Guid chatId);

    Task TakeChatAsync(Guid chatId, Guid adminId);

    Task CloseChatAsync(Guid chatId);
}