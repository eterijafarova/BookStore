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
        "Книга — это дверь. Вопрос лишь в том, решишься ли ты исчезнуть за ней.",
        "Все дороги ведут куда-то, но только книги знают, куда именно.",
        "Читать — это единственный способ исчезнуть и при этом остаться.",
        "Ты ищешь ответы? Забавно… книги давно ищут тебя.",
        "Если ты не знаешь, куда идти — открой книгу. Она знает дорогу.",
        "Книга — это сон, который ты выбираешь наяву.",
        "Некоторые страницы шепчут… но слышат их только те, кто готов читать.",
        "Чем глубже ты читаешь, тем меньше хочется возвращаться обратно."
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
}