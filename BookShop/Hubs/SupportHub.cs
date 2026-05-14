using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookShop.Hubs
{
    [Authorize] 
    public class SupportHub : Hub
    {
        // пользователь подключается к чату
        public async Task JoinChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        // отправка сообщения
        public async Task SendMessage(Guid chatId, string message)
        {
            var userId = Context.UserIdentifier;

            await Clients.Group(chatId.ToString())
                .SendAsync("ReceiveMessage", userId, message);
        }
    }
}