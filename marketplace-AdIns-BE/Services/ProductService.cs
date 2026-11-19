using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
/*using marketplace_AdIns_BE.Exceptions;
*/
namespace marketplace_AdIns_BE.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<ProductService> _logger;
        private object _bookingRepository;

        public ProductService(
            IRepository<Product> productRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<ProductResponse?> CreateAsync(ProductRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>?> GetAllAsync(int? sellerId)
        {
            var products = await _productRepository.GetAllAsync("Category", "Store");

            products = products.Where(p => p.Store.UserId == sellerId).ToList();

            if (!products.Any()) return null;

            return products;
        }

        public async Task<Product?> GetByIdAsync(long productId)
        {
            return await _productRepository.GetByIdAsync(productId, "Category", "Store");
        }

        public async Task<ProductResponse?> UpdateAsync(long productId, ProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return null;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            var updated = await GetByIdAsync(productId);
            return updated != null ? ToProductResponse(updated) : null;
        }
        public async Task<bool> DeleteAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;

            /*booking.Status = BookingStatus.CANCELLED;
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
            return true;*/
            throw new NotImplementedException();
        }

        public ProductResponse ToProductResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                StoreId = product.StoreId,
                StoreName = product.Store?.StoreName ?? string.Empty,
                CategoryName = product.Category?.CategoryName ?? string.Empty,
                ProductName = product.ProductName ?? string.Empty,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice ?? 0,
                ProductStock = product.ProductStock ?? 0,
                UpdatedAt = product.UpdatedAt ?? DateTime.Now,
                CreatedAt = product.CreatedAt ?? DateTime.Now,
            };
        }
    }
}
