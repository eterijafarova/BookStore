namespace BookShop.Data.Models;

public class PromoCode
{
    public int Id { get; set; }
    public string Code { get; set; } 
    public decimal Discount { get; set; }  
    public DateTime ExpiryDate { get; set; }  
    public bool IsActive { get; set; } 
    
    public ICollection<Order> Orders { get; set; }
}