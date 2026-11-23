using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
namespace marketplace_AdIns_BE.Services
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(long productId, long? sellerId = null);
        Task<List<Product>> GetAllAsync(string? search, long? sellerId, long? categoryId, int page, int limit);
        Task<ProductResponse?> CreateAsync(long sellerId, ProductRequest request);
        Task<ProductResponse?> UpdateAsync(long productId, long sellerId, ProductRequest request);
        Task<bool> DeleteAsync(long productId, long sellerId);
        ProductResponse ToProductResponse(Product product);
    }
}
