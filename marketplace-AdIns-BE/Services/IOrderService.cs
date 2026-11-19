using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
namespace marketplace_AdIns_BE.Services
{
    public interface IOrderService
    {
        Task<Order?> GetByIdAsync(long orderId);
        Task<List<Order?>> GetAllOrderForBuyerAsync(long buyerId);
        Task<List<Order?>> GetAllOrderForSellerAsync(long sellerId);
        Task<OrderResponse?> CreateAsync(OrderRequest request);
        Task<OrderResponse?> UpdateAsync(long orderId, OrderRequest request);
        Task<OrderResponse?> CalculateOrderAsync(long orderId);
        Task<bool> DeleteAsync(long productId);
        OrderResponse ToOrderResponse(Order order);
    }
}
