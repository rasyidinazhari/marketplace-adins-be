using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface IReviewService
    {
        Task<Review?> GetByIdAsync(long reviewId);
        Task<List<Review>> GetAllReviewByProductIdAsync(long productId);
        Task<ReviewResponse?> CreateAsync(ReviewRequest request);
        /*Task<ReviewResponse?> UpdateAsync(long productId, ReviewRequest request);
        Task<bool> DeleteAsync(long productId);*/
        ReviewResponse ToReviewResponse(Review review);
    }
}
