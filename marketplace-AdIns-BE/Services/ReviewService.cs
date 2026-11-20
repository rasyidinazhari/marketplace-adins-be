using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
/*using marketplace_AdIns_BE.Exceptions;
*/
namespace marketplace_AdIns_BE.Services
{
    public class ReviewService : IReviewService

    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IRepository<Review> reviewRepository,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public async Task<ReviewResponse?> CreateAsync(ReviewRequest request)
        {

            await _reviewRepository.BeginTransactionAsync();
            try
            {
                Review review = new()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Rating = request.Rating ?? 0,
                    comment = request.Comment ?? string.Empty,
                    Created_at = DateTime.UtcNow,
                    Updated_at = DateTime.UtcNow,
                };

                await _reviewRepository.AddAsync(review);

                await _reviewRepository.SaveChangesAsync();
                await _reviewRepository.CommitTransactionAsync();

/*                _logger.LogInformation($"Produk Cart berhasil di: Cart {cart.Id}, User {cart.UserId}, Product {cart.ProductId}");
*/                var created = await GetByIdAsync(review.Id);
                return created != null ? ToReviewResponse(created) : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Review>> GetAllReviewByProductIdAsync(long productId)
        {
            var reviews = await _reviewRepository.GetAllAsync("User", "Product");
            reviews = reviews.Where(r => r.ProductId == productId).ToList();

            if (!reviews.Any()) return null;

            return reviews;
            throw new NotImplementedException();
        }

        public async Task<Review?> GetByIdAsync(long reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) return null;

            return review;
        }

        public ReviewResponse ToReviewResponse(Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = review.Product?.ProductName ?? string.Empty,
                UserId = review.UserId,
                UserName = review.User?.Name ?? string.Empty,
                Rating = review.Rating,
                Comment = review.comment,
                UpdatedAt = review.Updated_at ?? DateTime.UtcNow,
                CreatedAt = review.Created_at ?? DateTime.UtcNow,
            };
        }
    }
}
