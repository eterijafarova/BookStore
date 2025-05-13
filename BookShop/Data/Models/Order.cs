using System.ComponentModel.DataAnnotations;
using BookShop.Auth.ModelsAuth;

namespace BookShop.Data.Models;

public class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public decimal OriginalPrice { get; set; }
    
    public string? PromoCode { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    [Required]
    public decimal FinalPrice { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public Guid UserAdressId { get; set; }
    public Adress UserAdress { get; set; } = null!;

    public Guid UserBankCardId { get; set; }
    public BankCard UserBankCard { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public enum OrderStatus { Pending, Paid, Shipped, Completed, Canceled }
}