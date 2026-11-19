using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
/*using marketplace_AdIns_BE.Exceptions;
*/
namespace marketplace_AdIns_BE.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly ILogger<OrderItemService> _logger;

        public OrderItemService(
            IRepository<OrderItem> orderItemRepository,
            ILogger<OrderItemService> logger)
        {
            _orderItemRepository = orderItemRepository;
            _logger = logger;
        }

        public async Task<OrderItemResponse?> CreateAsync(OrderItemRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(long orderItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Cart>> GetAllAsync(long buyerId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderItem?> GetByIdAsync(long orderItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderItemResponse?> UpdateAsync(long orderItemId, OrderItemRequest request)
        {
            throw new NotImplementedException();
        }

        public OrderItemResponse ToOrderItemResponse(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }
    }
}
