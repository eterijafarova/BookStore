using BookShop.Data.Models;

namespace BookShop.Services.Interfaces;

public interface IMessageService
{
    Task<Message> SendMessageAsync(
        Guid chatId,
        Guid senderId,
        string text);

    Task<List<Message>> GetMessagesAsync(
        Guid chatId);
}