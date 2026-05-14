namespace BookShop.Shared.DTO.Chat
{
    public class SendMessageDto
    {
        public Guid ChatId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }
    }
}