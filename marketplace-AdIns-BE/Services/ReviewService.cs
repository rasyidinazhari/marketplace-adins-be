using System.ComponentModel.DataAnnotations;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IRepository<Review> reviewRepository,
            IRepository<Order> orderRepository,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<ReviewResponse?> CreateAsync(ReviewRequest request)
        {
            // Validasi relasi pesanan jika OrderId disertakan
            if (request.OrderId.HasValue && request.OrderId.Value > 0)
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId.Value, "OrderItems");
                if (order == null || order.UserId != request.UserId)
                {
                    throw new ValidationException("Pesanan tidak ditemukan atau bukan milik pengguna ini.");
                }

                if (order.Status != OrderStatus.DELIVERED)
                {
                    throw new ValidationException("Ulasan hanya dapat diberikan untuk pesanan yang telah selesai (DELIVERED).");
                }

                if (!order.OrderItems.Any(oi => oi.ProductId == request.ProductId))
                {
                    throw new ValidationException("Produk ini tidak terdapat dalam pesanan tersebut.");
                }
            }

            // Validasi bahwa user hanya bisa memberikan ulasan 1 kali untuk produk ini
            var existingReviews = await _reviewRepository.GetAllAsync();
            if (existingReviews.Any(r => r.UserId == request.UserId && r.ProductId == request.ProductId))
            {
                throw new ValidationException("Ulasan untuk produk pada pesanan ini sudah pernah diberikan.");
            }

            await _reviewRepository.BeginTransactionAsync();
            try
            {
                Review review = new()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Rating = request.Rating ?? 5,
                    comment = request.Comment ?? string.Empty,
                    Created_at = DateTime.UtcNow,
                    Updated_at = DateTime.UtcNow,
                };

                await _reviewRepository.AddAsync(review);
                await _reviewRepository.SaveChangesAsync();
                await _reviewRepository.CommitTransactionAsync();

                var created = await GetByIdAsync(review.Id);
                return created != null ? ToReviewResponse(created) : null;
            }
            catch (Exception)
            {
                await _reviewRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<Review>> GetAllReviewByProductIdAsync(long productId)
        {
            var reviews = await _reviewRepository.GetAllAsync("User", "Product");
            var filtered = reviews.Where(r => r.ProductId == productId).ToList();
            return filtered;
        }

        public async Task<Review?> GetByIdAsync(long reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
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
