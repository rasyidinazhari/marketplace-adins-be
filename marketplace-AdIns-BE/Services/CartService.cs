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
                Cart cart = new()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Quantity = request.Quantity,
                    TotalPrice = request.TotalPrice,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _cartRepository.AddAsync(cart);

                await _cartRepository.SaveChangesAsync();
                await _cartRepository.CommitTransactionAsync();

                _logger.LogInformation($"Produk Cart berhasil di: Cart {cart.Id}, User {cart.UserId}, Product {cart.ProductId}");
                var created = await GetByIdAsync(cart.Id);
                return created != null ? ToCartResponse(created) : null;
            }
            catch (Exception ex)
            {
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

        public async Task<List<Cart>?> GetAllAsync(long buyerId)
        {
            var carts = await _cartRepository.GetAllAsync("User", "Product");
            carts = carts.Where(c => c.User?.Id == buyerId).ToList();

            if (!carts.Any()) return null;

            return carts;
            throw new NotImplementedException();
        }

        public async Task<Cart?> GetByIdAsync(long cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, "User", "Product");
            if (cart == null) return null;

            return cart;
        }

        public async Task<CartResponse?> UpdateAsync(long cartId, CartRequest request)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return null;

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
                UserId = cart.UserId,
                UserName = cart.User?.Name ?? string.Empty,
                Quantity = cart.Quantity,
                UpdatedAt = cart.UpdatedAt ?? DateTime.Now,
                CreatedAt = cart.CreatedAt ?? DateTime.Now,
            };
        }
    }
}
