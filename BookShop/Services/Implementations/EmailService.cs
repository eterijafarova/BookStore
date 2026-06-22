using System.Text;
using BookShop.Data.Contexts;
using BookShop.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace BookShop.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly LibraryContext _context;

    private static readonly List<string> CheshireQuotes =
    [
        "A book is a door. The only question is whether you dare to disappear behind it.",
        "All roads lead somewhere, but only books know exactly where.",
        "Reading is the only way to disappear and still remain.",
        "You’re looking for answers? Funny… books have been looking for you.",
        "If you don’t know where to go, open a book. It knows the way.",
        "A book is a dream you choose while awake.",
        "Some pages whisper… but only those ready to read can hear them",
        "The deeper you read, the less you want to return."
    ];

    public EmailService(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        LibraryContext context)
    {
        _configuration = configuration;
        _environment = environment;
        _context = context;
    }

    public async Task SendOrderReceiptAsync(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return;

        var random = new Random();

        var quote = CheshireQuotes[random.Next(CheshireQuotes.Count)];

        var templatePath = Path.Combine(
            _environment.WebRootPath,
            "templates",
            "order-receipt.html");

        var html = await File.ReadAllTextAsync(templatePath);

        var itemsHtml = new StringBuilder();

        foreach (var item in order.OrderItems)
        {
            itemsHtml.Append($"""
                <tr>
                    <td>{item.Book.Title}</td>
                    <td>{item.Quantity}</td>
                    <td>${item.Price}</td>
                </tr>
            """);
        }

        html = html
            .Replace("{{UserName}}", order.User.UserName)
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{OrderDate}}", order.OrderDate.ToString("dd.MM.yyyy"))
            .Replace("{{Total}}", order.FinalPrice.ToString("0.00"))
            .Replace("{{Discount}}", order.DiscountAmount.ToString("0.00"))
            .Replace("{{PromoCode}}", order.PromoCode ?? "None")
            .Replace("{{Items}}", itemsHtml.ToString())
            .Replace("{{Quote}}", quote);

        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(
            _configuration["Email:Smtp:User"]));

        email.To.Add(MailboxAddress.Parse(order.User.Email));

        email.Subject = "✨ Cheshire BookShop Receipt";

        email.Body = new TextPart("html")
        {
            Text = html
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _configuration["Email:Smtp:Host"],
            int.Parse(_configuration["Email:Smtp:Port"]),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _configuration["Email:Smtp:User"],
            _configuration["Email:Smtp:Pass"]);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
    
   public async Task SendOrderStatusChangedAsync(
    string email,
    string userName,
    Guid orderId,
    string status)
{
    var statusText = status switch
    {
        "Pending" =>
            "Your order is waiting between the shelves.",

        "Processing" =>
            "The librarians are preparing your books.",

        "Shipped" =>
            "Your order has escaped the library.",

        "Delivered" =>
            "The story has finally reached you.",

        "Cancelled" =>
            "This story vanished before completion.",

        _ =>
            "Your order status has changed."
    };

    var statusIcon = status switch
    {
        "Pending" => "⌛ Pending",
        "Processing" => "📚 Processing",
        "Shipped" => "📦 Shipped",
        "Delivered" => "🏠 Delivered",
        "Cancelled" => "❌ Cancelled",
        _ => "✨ Updated"
    };

    // Красивый номер отслеживания
    var trackingNumber =
        $"CBS-{orderId.ToString()[..8].ToUpper()}";
    
    Console.WriteLine($"TRACKING = {trackingNumber}");

    var templatePath = Path.Combine(
        _environment.WebRootPath,
        "templates",
        "order-status.html");

    var html = await File.ReadAllTextAsync(templatePath);

    html = html
        .Replace("{{UserName}}", userName)
        .Replace("{{Status}}", statusIcon)
        .Replace("{{StatusText}}", statusText)
        .Replace("{{TrackingNumber}}", trackingNumber);

    var message = new MimeMessage();

    message.From.Add(
        MailboxAddress.Parse(
            _configuration["Email:Smtp:User"]));

    message.To.Add(
        MailboxAddress.Parse(email));

    message.Subject = $"✨ Order {status}";

    message.Body = new TextPart("html")
    {
        Text = html
    };

    using var smtp = new SmtpClient();

    await smtp.ConnectAsync(
        _configuration["Email:Smtp:Host"],
        int.Parse(_configuration["Email:Smtp:Port"]),
        SecureSocketOptions.StartTls);

    await smtp.AuthenticateAsync(
        _configuration["Email:Smtp:User"],
        _configuration["Email:Smtp:Pass"]);

    await smtp.SendAsync(message);

    await smtp.DisconnectAsync(true);
}
}