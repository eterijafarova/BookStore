namespace BookShop.Services.Interfaces;

public interface IEmailService
{
    Task SendOrderReceiptAsync(Guid orderId);
    Task SendOrderStatusChangedAsync(
        string email,
        string userName,
        Guid orderId,
        string status);
}