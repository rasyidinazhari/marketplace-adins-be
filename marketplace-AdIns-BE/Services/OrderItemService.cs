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
            await _orderItemRepository.BeginTransactionAsync();

            try
            {
                OrderItem orderItem = new()
                {
                    OrderId = request.OrderId,
                    ProductId = request.ProductId,
                    PriceAtPurchase = request.PriceAtPurchase,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _orderItemRepository.AddAsync(orderItem);

                await _orderItemRepository.SaveChangesAsync();
                await _orderItemRepository.CommitTransactionAsync();

                /*                _logger.LogInformation($"Product berhasil dibuat: Product {product.Id}, Store {product.StoreId}, Product {product.ProductName}");
                */
                var created = await GetByIdAsync(orderItem.Id);
                return created != null ? ToOrderItemResponse(created) : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long orderItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderItem>> GetAllAsync(long orderId)
        {
            var orders = await _orderItemRepository.GetAllAsync("Order", "Product");

            orders = orders.Where(oi => oi.OrderId == orderId).ToList();

            if (!orders.Any()) return null;

            return orders;
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
            return new OrderItemResponse
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                ProductName = orderItem?.Product?.ProductName ?? string.Empty,
                UpdatedAt = orderItem?.UpdatedAt ?? DateTime.UtcNow,
                CreatedAt = orderItem?.CreatedAt ?? DateTime.UtcNow,
            };
        }
    }
}
