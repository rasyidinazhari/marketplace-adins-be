using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;

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
        public async Task<IActionResult> GetAllReviewsByProductId(long productId)
        {
            var reviews = await _reviewService.GetAllReviewByProductIdAsync(productId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview(
            long productId, [FromBody] ReviewRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized(new { status = "error", message = "Invalid or missing user ID claim." });
            }

            request.ProductId = productId;
            request.UserId = userId;

            try
            {
                ReviewResponse? result = await _reviewService.CreateAsync(request);
                if (result == null)
                {
                    return BadRequest(new { status = "error", message = "Gagal membuat ulasan." });
                }

                return Created($"/api/products/{productId}/reviews/{result.Id}", result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }
    }
}
