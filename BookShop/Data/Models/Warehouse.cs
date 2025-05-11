namespace BookShop.Data.Models;

public class Warehouse
{
    public int Id { get; set; }
    public Guid BookId { get; set; } 
    public int Quantity { get; set; } 
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 

    public Book Book { get; set; } = null!;
    
}