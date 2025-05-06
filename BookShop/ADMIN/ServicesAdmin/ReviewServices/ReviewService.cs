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

    public async Task AddReviewAsync(Guid userId, int bookId, string comment, int rating)
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


    // Получить все комментарии для книги
    public async Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId)
    {
        return await _context.Reviews
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    
    // Удалить комментарий по Id
    public async Task DeleteReviewByIdAsync(int reviewId, Guid userId, bool isAdmin = false)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            throw new Exception("Review not found.");
        }

        // Если не админ, проверяем, что пользователь пытается удалить только свой комментарий
        if (!isAdmin && review.UserId != userId)
        {
            throw new Exception("You can only delete your own reviews.");
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }

    
    // Удалить все комментарии (для администраторов)
    public async Task DeleteAllReviewsAsync()
    {
        var reviews = await _context.Reviews.ToListAsync();
        _context.Reviews.RemoveRange(reviews);
        await _context.SaveChangesAsync();
    }
}