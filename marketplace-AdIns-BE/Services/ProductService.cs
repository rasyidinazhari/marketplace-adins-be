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

        public ProductService(
            IRepository<Product> productRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<ProductResponse?> CreateAsync(ProductRequest request)
        {
            await _productRepository.BeginTransactionAsync();
            try
            {
                Product product = new()
                {
                    StoreId = request.StoreId,
                    CategoryId = request.CategoryId,
                    ProductName = request.ProductName,
                    ProductDescription = request.ProductDescription,
                    ProductPrice = request.ProductPrice,
                    ProductStock = request.ProductStock,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    isActive = true
                };

                await _productRepository.AddAsync(product);

                await _productRepository.SaveChangesAsync();
                await _productRepository.CommitTransactionAsync();

                _logger.LogInformation($"Product berhasil dibuat: Product {product.Id}, Store {product.StoreId}, Product {product.ProductName}");
                var created = await GetByIdAsync(product.Id);
                return created != null ? ToProductResponse(created) : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Product>?> GetAllAsync(int? sellerId)
        {
            var products = await _productRepository.GetAllAsync("Category", "Store");

            if (sellerId != null)
            {
                products = products.Where(p => p.Store.UserId == sellerId && p.isActive == true).ToList();
            } else
            {
                products = products.Where(p => p.ProductStock > 0).ToList();
            }

            if (!products.Any()) return null;

            return products;
        }

        public async Task<Product?> GetByIdAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId, "Category", "Store");

            if (product == null) return null;
            if (!(bool)product.isActive) return null;

            return product;
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

            product.isActive = false;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return true;
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
                ProductDescription = product.ProductDescription ?? string.Empty,
                ProductPrice = product.ProductPrice ?? 0,
                ProductStock = product.ProductStock ?? 0,
                UpdatedAt = product.UpdatedAt ?? DateTime.Now,
                CreatedAt = product.CreatedAt ?? DateTime.Now,
            };
        }
    }
}
