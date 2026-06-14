using BookShop.Data.Contexts;
using BookShop.Data.Enums;
using BookShop.Data.Models;
using BookShop.Hubs;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly LibraryContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageService(
        LibraryContext context,
        IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<Message> SendMessageAsync(
        Guid chatId,
        Guid senderId,
        string text)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new Exception("Chat not found");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Text = text,
            Type = MessageType.Text,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);

        await _context.SaveChangesAsync();

        await _hubContext.Clients
            .Group(chatId.ToString())
            .SendAsync("ReceiveMessage", new
            {
                message.Id,
                message.ChatId,
                message.SenderId,
                message.Text,
                message.SentAt
            });

        return message;
    }

    public async Task<List<Message>> GetMessagesAsync(
        Guid chatId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}