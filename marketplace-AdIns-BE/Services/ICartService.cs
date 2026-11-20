using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Services
{
    public interface ICartService
    {
        Task<Cart?> GetByIdAsync(long cartId);
        Task<List<CartResponse>?> GetAllAsync(long buyerId);
        Task<CartResponse?> CreateAsync(CartRequest request);
        Task<CartResponse?> UpdateAsync(long cartId, CartRequest request);
        Task<bool> DeleteAsync(long cartId);
        CartResponse ToCartResponse(Cart cart);
    }
}