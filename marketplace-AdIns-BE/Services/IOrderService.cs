using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetAllOrderForBuyerAsync(long buyerId);
        Task<List<OrderResponse>> GetAllOrderForSellerAsync(long sellerId);
        Task<OrderDetailResponse?> GetByIdAsync(long orderId, long userId, bool asSeller);
        Task<OrderDetailResponse> CreateAsync(long buyerId, OrderRequest request);
        Task<OrderResponse?> UpdateStatusAsync(long orderId, long userId, bool asSeller, OrderStatus status);
        OrderResponse ToOrderResponse(Order order);
        OrderDetailResponse ToOrderDetailResponse(Order order);
    }
}
