using BookShop.Data.Models;

namespace BookShop.ADMIN.ServicesAdmin.ReviewServices;

public interface IReviewService
{
    Task AddReviewAsync(Guid userId, Guid bookId, string comment, int rating);
    Task<IEnumerable<Review>> GetReviewsByBookAsync(Guid bookId);
    Task DeleteReviewByIdAsync(int reviewId, Guid userId,bool isAdmin = true);
    Task DeleteAllReviewsAsync();
}