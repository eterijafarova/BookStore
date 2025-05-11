using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Data.Models;

public class Book
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public int GenreId { get; set; }  
    public int? PublisherId { get; set; }  
    public string GenreName { get; set; } = string.Empty;
    public string PublisherName { get; set; } = string.Empty;

    public Genre Genre { get; set; } = null!;
    public Publisher Publisher { get; set; } = null!;



    public Warehouse? Warehouse { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<BookAttributeValue> BookAttributeValues { get; set; } = new List<BookAttributeValue>();
}