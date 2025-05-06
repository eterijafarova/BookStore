using System.ComponentModel.DataAnnotations;

namespace BookShop.ADMIN.DTOs;

public class ReviewDto
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
}