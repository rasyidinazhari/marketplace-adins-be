using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
/*using marketplace_AdIns_BE.Exceptions;
*/
namespace marketplace_AdIns_BE.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IRepository<Order> orderRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderResponse?> CalculateOrderAsync(long orderId)
        {
            var order = GetByIdAsync(orderId);

            
            throw new NotImplementedException();
        }

        public async Task<OrderResponse?> CreateAsync(OrderRequest request)
        {
            await _orderRepository.BeginTransactionAsync();
            
            try
            {
                Order order = new()
                {
                    UserId = request.UserId,
                    StoreId = request.StoreId,
                    AddressId = request.AddressId,
                    VoucherId = request.VoucherId ?? null,  
                    DiscountAmount = request.DiscountAmount ?? 0,
                    TotalBeforeDisc = request.TotalBeforeDisc ?? 0,
                    TotalAfterDisc = request.TotalAfterDisc ?? 0,
                    Status = OrderStatus.PENDING_PAYMENT,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _orderRepository.AddAsync(order);

                await _orderRepository.SaveChangesAsync();
                await _orderRepository.CommitTransactionAsync();

/*                _logger.LogInformation($"Product berhasil dibuat: Product {product.Id}, Store {product.StoreId}, Product {product.ProductName}");
*/              var created = await GetByIdAsync(order.Id);
                return created != null ? ToOrderResponse(created) : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long productId)
        {
            throw new NotImplementedException();
        }

        public async Task<Order?> GetByIdAsync(long orderId)
        {

            var order = await _orderRepository.GetByIdAsync(orderId, "User", "Store", "Voucher", "Address");

            if (order == null) return null;

            return order;
        }


        public async Task<OrderResponse?> UpdateAsync(long orderId, OrderRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            var updated = await GetByIdAsync(orderId);
            return updated != null ? ToOrderResponse(updated) : null;
        }
        
        public async Task<List<Order?>> GetAllOrderForBuyerAsync(long buyerId)
        {
            var orders = await _orderRepository.GetAllAsync("User", "Store", "Voucher", "Address");
/*            var orders = await _orderRepository.GetAllAsync();
*/
            orders = orders.Where(o => o.UserId == buyerId).ToList();

            if (!orders.Any()) return null;

            return orders; 
        }

        public async Task<List<Order?>> GetAllOrderForSellerAsync(long sellerId)
        {
            var orders = await _orderRepository.GetAllAsync("User", "Store", "Voucher", "Address");

            orders = orders.Where(o => o.UserId == sellerId).ToList();

            if (!orders.Any()) return null;

            return orders;
        }

        public OrderResponse ToOrderResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order?.User?.Name ?? string.Empty,
                StoreId = order.StoreId,
                StoreName = order.Store?.StoreName ?? string.Empty,
                AddressId = order.AddressId,
                DiscountAmount = order.DiscountAmount,
                RecipientName = order?.Address?.User?.Name ?? string.Empty,
                TotalAfterDisc = order?.TotalAfterDisc,
                TotalBeforeDisc = order.TotalBeforeDisc,
                VoucherId = order.VoucherId,
                VoucherCode = order?.Voucher?.Code ?? string.Empty,
                UpdatedAt = order?.UpdatedAt ?? DateTime.Now,
                CreatedAt = order?.CreatedAt ?? DateTime.Now,
            };
        }
    }
}
