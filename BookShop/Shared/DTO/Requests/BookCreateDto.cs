namespace BookShop.Shared.DTO.Requests;

public class BookCreateDto
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; } = "";
    public int GenreId { get; set; }
    public int? PublisherId { get; set; }
  
}
