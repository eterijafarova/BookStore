using BookShop.Data.Models;

namespace BookShop.ADMIN.ServicesAdmin.ReviewServices;

public interface IReviewService
{
    Task AddReviewAsync(Guid userId, int bookId, string comment, int rating);
    Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId);
    Task DeleteReviewByIdAsync(int reviewId, Guid userId,bool isAdmin = true);
    Task DeleteAllReviewsAsync();
}