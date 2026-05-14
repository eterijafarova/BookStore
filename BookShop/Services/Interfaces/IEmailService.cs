namespace BookShop.Services.Interfaces;

public interface IEmailService
{
    Task SendOrderReceiptAsync(Guid orderId);
}