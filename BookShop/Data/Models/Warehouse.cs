namespace BookShop.Data.Models;

public class Warehouse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookId { get; set; } // ✅ Изменено на Guid, чтобы соответствовать Book.Id
    public int Quantity { get; set; } // Количество в наличии
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Дата последнего обновления

    public Book Book { get; set; } = null!;


    // Метод для обновления количества и времени последнего изменения
    public void UpdateStock(int amount)
    {
        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }
}