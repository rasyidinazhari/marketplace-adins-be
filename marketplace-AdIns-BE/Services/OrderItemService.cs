using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;

namespace marketplace_AdIns_BE.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IRepository<OrderItem> _orderItemRepository;

        public OrderItemService(IRepository<OrderItem> orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<OrderItemResponse?> CreateAsync(OrderItemRequest request)
        {
            await _orderItemRepository.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var item = new OrderItem
                {
                    OrderId = request.OrderId,
                    ProductId = request.ProductId,
                    PriceAtPurchase = request.PriceAtPurchase,
                    Quantity = request.Quantity,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _orderItemRepository.AddAsync(item);
                await _orderItemRepository.SaveChangesAsync();
                var saved = await GetByIdAsync(item.Id)
                    ?? throw new InvalidOperationException("Item pesanan gagal dimuat.");
                var response = ToOrderItemResponse(saved);
                await _orderItemRepository.CommitTransactionAsync();
                return response;
            }
            catch
            {
                await _orderItemRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<OrderItem>> GetAllAsync(long orderId)
        {
            var items = await _orderItemRepository.GetAllAsync("Product");
            return items.Where(item => item.OrderId == orderId).ToList();
        }

        public async Task<OrderItem?> GetByIdAsync(long orderItemId)
        {
            return await _orderItemRepository.GetByIdAsync(orderItemId, "Product");
        }

        public Task<OrderItemResponse?> UpdateAsync(long orderItemId, OrderItemRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(long orderItemId)
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
                ProductName = orderItem.Product?.ProductName ?? string.Empty,
                Quantity = orderItem.Quantity,
                PriceAtPurchase = orderItem.PriceAtPurchase,
                CreatedAt = orderItem.CreatedAt,
                UpdatedAt = orderItem.UpdatedAt
            };
        }
    }
}
