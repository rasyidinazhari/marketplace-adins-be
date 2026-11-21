using System.ComponentModel.DataAnnotations;
using System.Data;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace marketplace_AdIns_BE.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IOrderItemService _orderItemService;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<Cart> cartRepository,
            IRepository<Address> addressRepository,
            IRepository<Voucher> voucherRepository,
            IRepository<Store> storeRepository,
            IOrderItemService orderItemService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _voucherRepository = voucherRepository;
            _storeRepository = storeRepository;
            _orderItemService = orderItemService;
        }

        private bool CanAccessOrder(Order order, long userId, bool asSeller)
        {
            return asSeller ? order.Store?.UserId == userId : order.UserId == userId;
        }

        public Task<List<OrderResponse>> GetAllOrderForBuyerAsync(long buyerId)
        {
            return GetOrdersAsync(buyerId, asSeller: false);
        }

        public Task<List<OrderResponse>> GetAllOrderForSellerAsync(long sellerId)
        {
            return GetOrdersAsync(sellerId, asSeller: true);
        }

        private async Task<List<OrderResponse>> GetOrdersAsync(long userId, bool asSeller)
        {
            var orders = await _orderRepository.GetAllAsync("User", "Store", "Address", "Voucher");
            return orders.Where(order => CanAccessOrder(order, userId, asSeller))
                .OrderByDescending(order => order.Id).Select(ToOrderResponse).ToList();
        }

        public async Task<OrderDetailResponse?> GetByIdAsync(long orderId, long userId, bool asSeller)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, "User", "Store", "Address", "Voucher", "OrderItems.Product");
            if (order == null || !CanAccessOrder(order, userId, asSeller)) return null;
            return ToOrderDetailResponse(order);
        }

        public async Task<OrderDetailResponse> CreateAsync(long buyerId, OrderRequest request)
        {
            await _orderRepository.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                var address = await _addressRepository.GetByIdAsync(request.AddressId, "User.Carts.Product.Store");
                if (address?.User == null || address.UserId != buyerId)
                    throw new ValidationException("Alamat pengiriman tidak tersedia untuk pembeli ini.");

                var cart = address.User.Carts.Where(c => c.Product?.StoreId == request.StoreId)
                    .OrderBy(c => c.ProductId).ThenBy(c => c.Id).ToList();
                if (cart.Count == 0) throw new ValidationException("Keranjang untuk toko ini kosong.");

                var now = DateTime.UtcNow;
                var order = new Order
                {
                    UserId = buyerId,
                    User = address.User,
                    Address = address,
                    Store = cart[0].Product!.Store,
                    StoreId = request.StoreId,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                foreach (var row in cart)
                {
                    var product = row.Product!;
                    if (row.Quantity <= 0 || product.isActive != true || product.ProductPrice is null or < 0)
                        throw new ValidationException("Keranjang memuat produk atau jumlah yang tidak valid.");
                    if ((product.ProductStock ?? 0) < row.Quantity)
                        throw new ValidationException($"Stok {product.ProductName} tidak mencukupi.");

                    order.OrderItems.Add(new OrderItem
                    {
                        Product = product,
                        Quantity = row.Quantity,
                        PriceAtPurchase = product.ProductPrice.Value,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                    order.TotalBeforeDisc += product.ProductPrice.Value * row.Quantity;
                    product.ProductStock -= row.Quantity;
                    product.UpdatedAt = now;
                }

                if (order.TotalBeforeDisc > 9999999999.99m)
                    throw new ValidationException("Total pesanan melebihi batas yang dapat disimpan.");

                order.Voucher = await GetVoucherAsync(request, order.TotalBeforeDisc);
                
                if (order.Voucher != null)
                {
                    var discount = decimal.Round(order.TotalBeforeDisc * order.Voucher.DiscountPercentage / 100m,
                        2, MidpointRounding.AwayFromZero);
                    order.DiscountAmount = Math.Min(discount, order.Voucher.MaxDiscountAmount);
                }
                order.TotalAfterDisc = order.TotalBeforeDisc - order.DiscountAmount;
                await _orderRepository.AddAsync(order);
                foreach (var row in cart)
                {
                    _cartRepository.Delete(row);
                }
                await _orderRepository.SaveChangesAsync();
                await _orderRepository.CommitTransactionAsync();
                return ToOrderDetailResponse(order);
            }
            catch (Exception ex)
            {
                await _orderRepository.RollbackTransactionAsync();
                if (ex is DbUpdateConcurrencyException ||
                    ex.GetBaseException() is PostgresException { SqlState: "40001" or "40P01" })
                {
                    throw new DBConcurrencyException("Data pesanan berubah. Muat ulang sebelum mencoba lagi.", ex);
                }
                throw;
            }
        }

        private async Task<Voucher?> GetVoucherAsync(OrderRequest request, decimal subtotal)
        {
            var code = request.VoucherCode?.Trim();
            if (request.VoucherId == null && string.IsNullOrEmpty(code)) return null;

            Voucher? voucher;
            if (request.VoucherId.HasValue)
            {
                voucher = await _voucherRepository.GetByIdAsync(request.VoucherId.Value);
            }
            else
            {
                var store = await _storeRepository.GetByIdAsync(request.StoreId, "Vouchers");
                voucher = store?.Vouchers.FirstOrDefault(v => v.Code == code);
            }
            if (voucher == null || voucher.StoreId != request.StoreId || !voucher.IsActive || (!string.IsNullOrEmpty(code) && voucher.Code != code)
                || voucher.DiscountPercentage is < 0 or > 100 || voucher.MaxDiscountAmount < 0)
                throw new ValidationException("Voucher tidak valid atau tidak aktif.");
            if (subtotal < voucher.MinPurchaseAmount)
                throw new ValidationException("Subtotal belum memenuhi minimum pembelian voucher.");
            return voucher;
        }

        public async Task<OrderResponse?> UpdateStatusAsync(long orderId, long userId, bool asSeller, OrderStatus status)
        {
            await _orderRepository.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId, "User", "Store", "Address", "Voucher");
                if (order == null || !CanAccessOrder(order, userId, asSeller))
                {
                    await _orderRepository.CommitTransactionAsync();
                    return null;
                }

                var allowed = asSeller
                    ? status == OrderStatus.ON_DELIVERY
                    : status is OrderStatus.ON_PROCESS or OrderStatus.DELIVERED;
                if (!allowed) throw new UnauthorizedAccessException();
                if (order.Status == status)
                {
                    await _orderRepository.CommitTransactionAsync();
                    return ToOrderResponse(order);
                }

                var nextStatus = order.Status switch
                {
                    OrderStatus.PENDING_PAYMENT => OrderStatus.ON_PROCESS,
                    OrderStatus.ON_PROCESS => OrderStatus.ON_DELIVERY,
                    OrderStatus.ON_DELIVERY => OrderStatus.DELIVERED,
                    _ => (OrderStatus?)null
                };
                if (status != nextStatus)
                    throw new DBConcurrencyException("Perubahan status tidak sesuai urutan pesanan.");

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.SaveChangesAsync();
                await _orderRepository.CommitTransactionAsync();
                return ToOrderResponse(order);
            }
            catch (Exception ex)
            {
                await _orderRepository.RollbackTransactionAsync();
                if (ex is DbUpdateConcurrencyException ||
                    ex.GetBaseException() is PostgresException { SqlState: "40001" or "40P01" })
                {
                    throw new DBConcurrencyException("Data pesanan berubah. Muat ulang sebelum mencoba lagi.", ex);
                }
                throw;
            }
        }

        public OrderResponse ToOrderResponse(Order order)
        {
            var response = new OrderResponse();
            SetOrderFields(order, response);
            return response;
        }

        public OrderDetailResponse ToOrderDetailResponse(Order order)
        {
            var response = new OrderDetailResponse
            {
                Items = order.OrderItems.OrderBy(item => item.Id)
                    .Select(_orderItemService.ToOrderItemResponse).ToList()
            };
            SetOrderFields(order, response);
            return response;
        }

        private void SetOrderFields(Order order, OrderResponse response)
        {
            response.Id = order.Id;
            response.UserId = order.UserId;
            response.UserName = order.User?.Name ?? string.Empty;
            response.StoreId = order.StoreId;
            response.StoreName = order.Store?.StoreName ?? string.Empty;
            response.AddressId = order.AddressId;
            response.RecipientName = order.Address?.RecipientName ?? string.Empty;
            response.VoucherId = order.VoucherId;
            response.VoucherCode = order.Voucher?.Code;
            response.DiscountAmount = order.DiscountAmount;
            response.TotalBeforeDisc = order.TotalBeforeDisc;
            response.TotalAfterDisc = order.TotalAfterDisc;
            response.OrderStatus = order.Status;
            response.CreatedAt = order.CreatedAt;
            response.UpdatedAt = order.UpdatedAt;
        }
    }
}
