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
                    /*CategoryId = request.CategoryId,*/
                    ProductName = request.ProductName,
                    ProductDescription = request.ProductDescription,
                    ProductPrice = request.ProductPrice,
                    ProductStock = request.ProductStock,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    isActive = true
                };

                product.ProductCategories = new List<ProductCategory>();


                /*await _bookingRepository.AddAsync(booking); --> Ini tidak dipakai */

                foreach (var categoryId in request.CategoryIds)
                {
                    /*seat.IsAvailable = false;
                    _seatRepository.Update(seat);*/

                    /*await _bookingSeatRepository.AddAsync(new BookingSeat
                    {
                        BookingId = booking.Id,
                        SeatId = seat.Id
                    });*/

                    ProductCategory newProductCategory = new ProductCategory();
                    newProductCategory.Product = product;
                    newProductCategory.CategoryId = categoryId;

                    product.ProductCategories.Add(newProductCategory);
                }

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

        public async Task<List<Product>?> GetAllAsync(string? search, long? sellerId, long? categoryId, int page, int limit)
        {
            var products = await _productRepository.GetAllAsync("Store", "ProductCategories");

            // For Seller
            if (sellerId != null)
            {
                // Filter by Seller
                products = products.Where(p => p.Store != null && p.Store.UserId == sellerId && p.isActive == true).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.ProductName != null && p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Filter by Category 
                if (categoryId.HasValue && categoryId.Value != 0)
                {
                    // Gunakan .Any() untuk mengecek apakah ada kategori yang cocok di dalam list
                    products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value)).ToList();
                }
            }
            else // For Buyer
            {
                // Filter dasar untuk Buyer
                products = products.Where(p => p.isActive == true && p.ProductStock > 0).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.ProductName != null && p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Filter by Category untuk Buyer
                if (categoryId.HasValue && categoryId.Value != 0)
                {
                    products = products.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value)).ToList();
                }
            }

            
            if (!products.Any()) return new List<Product>();

            return products.Skip((page - 1) * limit).Take(limit).ToList();
        }

        public async Task<Product?> GetByIdAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId, "ProductCategories.Category", "Store");

            if (product == null) return null;
            if (!(bool)product.isActive) return null;

            return product;
        }

        public async Task<ProductResponse?> UpdateAsync(long productId, ProductRequest request)
        {
            var products = await _productRepository.GetAllAsync();
            var product = products.FirstOrDefault(p => p.Id == productId && p.StoreId == request.StoreId);
            if (product == null) return null;

            if (request.ProductName != null)
            {
                product.ProductName = request.ProductName;
            } 
            if (request.ProductDescription != null)
            {
                product.ProductDescription = request.ProductDescription;
            } 
            if (request.ProductPrice != null)
            {
                product.ProductPrice = request.ProductPrice;
            } 
            if (request.ProductStock != null)
            {
                product.ProductStock = request.ProductStock;
            }

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
                /*CategoryId = product.ProductCategories.FirstOrDefault()?.CategoryId ?? 0,
                CategoryName = product.ProductCategories.FirstOrDefault()?.Category?.CategoryName ?? string.Empty,*/
                Categories = product.ProductCategories
                    .Where(pc => pc.Category != null && pc.ProductId == product.Id)
                    .Select(pc => pc.Category!)
                    .ToList(),
                StoreId = product.StoreId,
                StoreName = product.Store?.StoreName ?? string.Empty,
                ProductName = product.ProductName ?? string.Empty,
                ProductDescription = product.ProductDescription ?? string.Empty,
                ProductPrice = product.ProductPrice ?? 0,
                ProductStock = product.ProductStock ?? 0,
                UpdatedAt = product.UpdatedAt ?? DateTime.UtcNow,
                CreatedAt = product.CreatedAt ?? DateTime.UtcNow,
            };
        }
    }
}
