using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
namespace marketplace_AdIns_BE.Services
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(long productId);
        Task<List<Product>> GetAllAsync(string? search, int? SellerId, int page, int limit);
        Task<ProductResponse?> CreateAsync(ProductRequest request);
        Task<ProductResponse?> UpdateAsync(long productId, ProductRequest request);
        Task<bool> DeleteAsync(long productId);
        ProductResponse ToProductResponse(Product product);
    }
}
