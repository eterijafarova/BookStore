using BookShop.Data.Models;

namespace BookShop.Services.Interfaces
{
    public interface IChatService
    {
        Task<Chat> CreateChatAsync(int userId);
        Task<Message> SendMessageAsync(Guid chatId, int senderId, string text);
        Task AssignAdminAsync(Guid chatId, int adminId);
        Task CloseChatAsync(Guid chatId);
    }
    }