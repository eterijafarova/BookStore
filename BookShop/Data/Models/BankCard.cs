using BookShop.Auth.ModelsAuth;

namespace BookShop.Data.Models;

public class BankCard
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } 
    public string CardHolderName { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string CVV { get; set; } 
    
    public string Last4Digits { get; set; }
    public Guid UserId { get; set; } 
    
    public User User { get; set; }
}