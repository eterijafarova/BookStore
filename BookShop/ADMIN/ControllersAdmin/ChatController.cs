using BookShop.Data.Contexts;
using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IMessageService _messageService;
    private readonly LibraryContext _context;

    public ChatController(
        IChatService chatService,
        IMessageService messageService,
        LibraryContext context)
    {
        _chatService = chatService;
        _messageService = messageService;
        _context = context;
    }

    private async Task<Guid> GetCurrentUserId()
    {
        var userName = User.Identity?.Name;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
            throw new Exception("User not found");

        return user.Id;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat()
    {
        var userId = await GetCurrentUserId();

        var chat = await _chatService.CreateChatAsync(userId);

        return Ok(chat);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyChats()
    {
        var userId = await GetCurrentUserId();

        var chats = await _chatService.GetUserChatsAsync(userId);

        return Ok(chats);
    }

    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChat(Guid chatId)
    {
        var chat = await _chatService.GetChatByIdAsync(chatId);

        if (chat == null)
            return NotFound();

        return Ok(chat);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingChats()
    {
        var chats = await _chatService.GetWaitingChatsAsync();

        return Ok(chats);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("take/{chatId}")]
    public async Task<IActionResult> TakeChat(Guid chatId)
    {
        var adminId = await GetCurrentUserId();

        await _chatService.TakeChatAsync(chatId, adminId);

        return Ok("Chat accepted");
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("close/{chatId}")]
    public async Task<IActionResult> CloseChat(Guid chatId)
    {
        await _chatService.CloseChatAsync(chatId);

        return Ok("Chat closed");
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageRequest request)
    {
        var senderId = await GetCurrentUserId();

        var message = await _messageService.SendMessageAsync(
            request.ChatId,
            senderId,
            request.Text);

        return Ok(message);
    }

    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        var messages = await _messageService.GetMessagesAsync(chatId);

        return Ok(messages);
    }
}