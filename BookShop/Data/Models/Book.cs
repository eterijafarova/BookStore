﻿namespace BookShop.Data.Models;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Author { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public Guid GenreId { get; set; }
    public Guid PublisherId { get; set; }

    public decimal? Discount { get; set; } // скидка, если есть
    public Genre Genre { get; set; }
    public Publisher Publisher { get; set; }
    public Warehouse Warehouse { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    
    public ICollection<BookAttributeValue> BookAttributeValues { get; set; } = new List<BookAttributeValue>();

}
