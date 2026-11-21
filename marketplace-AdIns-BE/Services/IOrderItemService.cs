using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Services
{
    public interface IOrderItemService
    {
        Task<OrderItem?> GetByIdAsync(long orderItemId);
        Task<List<OrderItemResponse>> GetAllAsync(long orderId);
        Task<OrderItemResponse?> CreateAsync(OrderItemRequest request);
        Task<OrderItemResponse?> UpdateAsync(long orderItemId, OrderItemRequest request);
        Task<bool> DeleteAsync(long orderItemId);
        OrderItemResponse ToOrderItemResponse(OrderItem orderItem);
    }
}
