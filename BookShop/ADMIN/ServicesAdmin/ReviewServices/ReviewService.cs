using BookShop.Data.Contexts;
using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.ADMIN.ServicesAdmin.ReviewServices;

public class ReviewService : IReviewService
{
    private readonly LibraryContext _context;

    public ReviewService(LibraryContext context)
    {
        _context = context;
    }

    public async Task AddReviewAsync(Guid userId, Guid bookId, string comment, int rating)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.");
        }

        if (string.IsNullOrEmpty(comment))
        {
            throw new ArgumentException("Comment cannot be null or empty.");
        }

        // Логика добавления отзыва
        var review = new Review
        {
            UserId = userId,
            BookId = bookId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    
    public async Task<IEnumerable<Review>> GetReviewsByBookAsync(Guid bookId)
    {
        return await _context.Reviews
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    public async Task DeleteReviewByIdAsync(int reviewId, Guid userId, bool isAdmin = false)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            throw new Exception("Review not found.");
        }

       
        if (!isAdmin && review.UserId != userId)
        {
            throw new Exception("You can only delete your own reviews.");
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }

    
    public async Task DeleteAllReviewsAsync()
    {
        var reviews = await _context.Reviews.ToListAsync();
        _context.Reviews.RemoveRange(reviews);
        await _context.SaveChangesAsync();
    }
}