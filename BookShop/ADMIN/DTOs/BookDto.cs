using System.ComponentModel.DataAnnotations;

namespace BookShop.ADMIN.DTOs
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string GenreName { get; set; }  
        public string PublisherName { get; set; }  
        public int GenreId { get; set; }
    }

    public class CreateBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int GenreId { get; set; }
        public int? PublisherId { get; set; }
    }

    public class UpdateBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int GenreId { get; set; }
        public int? PublisherId { get; set; }
    }
    
    public class UpdatePriceDto
    {
        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
        public decimal Price { get; set; }
    }
    
    public class UpdateStockDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}