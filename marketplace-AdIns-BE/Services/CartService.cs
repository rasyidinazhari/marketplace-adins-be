using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
/*using marketplace_AdIns_BE.Exceptions;
*/
namespace marketplace_AdIns_BE.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(
            IRepository<Cart> cartRepository,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }
        public async Task<CartResponse?> CreateAsync(CartRequest request)
        {
            await _cartRepository.BeginTransactionAsync();
            try
            {
                var existingCarts = await _cartRepository.GetAllAsync();
                var existingCart = existingCarts.FirstOrDefault(c => c.UserId == request.UserId && c.ProductId == request.ProductId);

                if (existingCart != null)
                {
                    existingCart.Quantity += request.Quantity;
                    existingCart.TotalPrice += request.TotalPrice;
                    existingCart.UpdatedAt = DateTime.UtcNow;

                    _cartRepository.Update(existingCart);
                    await _cartRepository.SaveChangesAsync();
                    await _cartRepository.CommitTransactionAsync();

                    _logger.LogInformation($"Kuantitas Cart berhasil diperbarui: Cart {existingCart.Id}, User {existingCart.UserId}, Product {existingCart.ProductId}, Qty {existingCart.Quantity}");
                    var updated = await GetByIdAsync(existingCart.Id);
                    return updated != null ? ToCartResponse(updated) : null;
                }

                Cart cart = new()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Quantity = request.Quantity,
                    TotalPrice = request.TotalPrice,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _cartRepository.AddAsync(cart);

                await _cartRepository.SaveChangesAsync();
                await _cartRepository.CommitTransactionAsync();

                _logger.LogInformation($"Produk Cart berhasil dibuat: Cart {cart.Id}, User {cart.UserId}, Product {cart.ProductId}");
                var created = await GetByIdAsync(cart.Id);
                return created != null ? ToCartResponse(created) : null;
            }
            catch (Exception)
            {
                await _cartRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return false;

            _cartRepository.Delete(cart);
            await _cartRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<CartResponse>?> GetAllAsync(long buyerId)
        {
            // Tambahkan "Product.Store"
            var carts = await _cartRepository.GetAllAsync("User", "Product", "Product.Store");

            carts = carts.Where(c => c.User?.Id == buyerId).ToList();

            if (!carts.Any()) return null;

            var cartResponses = carts.Select(c => ToCartResponse(c)).ToList();

            return cartResponses;
        }

        public async Task<Cart?> GetByIdAsync(long cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, "User", "Product", "Product.Store");
            if (cart == null) return null;

            return cart;
        }

        public async Task<CartResponse?> UpdateAsync(long cartId, CartRequest request)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return null;

            if (request.Quantity != null)
            {
                cart.Quantity = request.Quantity;
            }
            if (request.TotalPrice != null)
            {
                cart.TotalPrice = request.TotalPrice;
            }

            _cartRepository.Update(cart);
            await _cartRepository.SaveChangesAsync();

            var updated = await GetByIdAsync(cartId);
            return updated != null ? ToCartResponse(updated) : null;
        }

        public CartResponse ToCartResponse(Cart cart)
        {
            return new CartResponse
            {
                Id = cart.Id,
                ProductId = cart.ProductId,
                ProductName = cart.Product?.ProductName ?? string.Empty,
                StoreId = (long)(cart.Product?.StoreId),
                StoreName = cart.Product?.Store?.StoreName ?? string.Empty,
                UserId = cart.UserId,
                UserName = cart.User?.Name ?? string.Empty,
                Quantity = cart.Quantity,
                TotalPrice = cart.TotalPrice,
                CurrentTotalPrice = cart.Product?.ProductPrice * cart.Quantity,
                UpdatedAt = cart.UpdatedAt ?? DateTime.UtcNow,
                CreatedAt = cart.CreatedAt ?? DateTime.UtcNow,
            };
        }
    }
}
