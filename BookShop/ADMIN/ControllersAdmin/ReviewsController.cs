using System.Security.Claims;
using BookShop.ADMIN.DTOs;
using BookShop.ADMIN.ServicesAdmin.ReviewServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "User, Admin, SuperAdmin")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("add/{bookId}")]
    public async Task<IActionResult> AddReview(Guid bookId, [FromBody] AddReviewRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            await _reviewService.AddReviewAsync(Guid.Parse(userId), bookId, request.Comment, request.Rating);
            return Ok(new { message = "Review added successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }



    [HttpGet("get-by-book/{bookId}")]
    public async Task<IActionResult> GetReviewsByBook(Guid bookId)
    {
        var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
        return Ok(reviews);
    }

    // Удалить свой комментарий
    [HttpDelete("delete/{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        try
        {
            await _reviewService.DeleteReviewByIdAsync(reviewId, userId);
            return Ok(new { message = "Review deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    

    // Удалить все комментарии (только для администраторов)
    [HttpDelete("delete-all")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<IActionResult> DeleteAllReviews()
    {
        try
        {
            await _reviewService.DeleteAllReviewsAsync();
            return Ok(new { message = "All reviews deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

