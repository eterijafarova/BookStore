using BookShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChat(int userId)
        {
            var chat = await _chatService.CreateChatAsync(userId);
            return Ok(chat);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("assign")]
        public async Task<IActionResult> Assign(Guid chatId, int adminId)
        {
            await _chatService.AssignAdminAsync(chatId, adminId);
            return Ok();
        }

        [HttpPost("close")]
        public async Task<IActionResult> Close(Guid chatId)
        {
            await _chatService.CloseChatAsync(chatId);
            return Ok();
        }
    }
}