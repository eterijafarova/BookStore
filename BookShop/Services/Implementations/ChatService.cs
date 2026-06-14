using BookShop.Data.Contexts;
using BookShop.Data.Enums;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Services.Implementations;

public class ChatService : IChatService
{
    private readonly LibraryContext _context;

    public ChatService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<Chat> CreateChatAsync(Guid userId)
    {
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = ChatStatus.Waiting,
            CreatedAt = DateTime.UtcNow
        };

        _context.Chats.Add(chat);

        await _context.SaveChangesAsync();

        return chat;
    }

    public async Task<List<Chat>> GetWaitingChatsAsync()
    {
        return await _context.Chats
            .Include(c => c.User)
            .Where(c => c.Status == ChatStatus.Waiting)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Chat>> GetUserChatsAsync(Guid userId)
    {
        return await _context.Chats
            .Include(c => c.Messages)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task TakeChatAsync(Guid chatId, Guid adminId)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new Exception("Chat not found");

        if (chat.AdminId != null)
            throw new Exception("Chat already taken");

        chat.AdminId = adminId;
        chat.Status = ChatStatus.InProgress;

        await _context.SaveChangesAsync();
    }

    public async Task CloseChatAsync(Guid chatId)
    {
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new Exception("Chat not found");

        chat.Status = ChatStatus.Closed;
        chat.ClosedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<Chat?> GetChatByIdAsync(Guid chatId)
    {
        return await _context.Chats
            .Include(c => c.User)
            .Include(c => c.Admin)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }
}