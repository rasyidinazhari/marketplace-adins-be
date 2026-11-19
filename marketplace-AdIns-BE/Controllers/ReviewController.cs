using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/products/{productId:long}/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(long productId)
        {
            var reviews = await _reviewService.GetByProductAsync(productId);
            return Ok(reviews);
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost]
        public async Task<IActionResult> CreateReview(
            long productId, [FromBody] CreateReviewRequest request)
        {
            long userId = long.Parse(User.FindFirst("userId")!.Value);
            try
            {
                ReviewDetailResponse? result = await _reviewService.CreateAsync(userId, productId, request);
                return Created($"/api/{productId}/reviews/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
    }
}
