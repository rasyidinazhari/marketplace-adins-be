using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface IReviewService
    {
        Task<Review?> GetByIdAsync(long productId);
        Task<List<Review>> GetAllByProductIdAsync(long productId);
        Task<ReviewResponse?> CreateAsync(Review request);
        Task<ReviewResponse?> UpdateAsync(long productId, ReviewRequest request);
        Task<bool> DeleteAsync(long productId);
        ReviewResponse ToReviewResponse(Review review);
    }
}
