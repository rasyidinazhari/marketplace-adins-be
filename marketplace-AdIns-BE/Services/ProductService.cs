using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace marketplace_AdIns_BE.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IRepository<Product> productRepository,
            IRepository<Store> storeRepository,
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _logger = logger;
        }
        public async Task<ProductResponse?> CreateAsync(long sellerId, ProductRequest request)
        {
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null || store.UserId != sellerId) return null;

            var productName = request.ProductName?.Trim();
            if (string.IsNullOrEmpty(productName) || productName.Length < 5)
                throw new ValidationException("Nama produk minimal 5 karakter.");

            if (request.ProductPrice == null || request.ProductStock == null)
                throw new ValidationException("Harga dan stok produk wajib diisi.");

            var categories = await GetCategoriesAsync(request.CategoryIds);

            await _productRepository.BeginTransactionAsync();
            try
            {
                Product product = new()
                {
                    StoreId = request.StoreId,
                    ProductName = productName,
                    ProductDescription = request.ProductDescription,
                    ProductPrice = request.ProductPrice,
                    ProductStock = request.ProductStock,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    isActive = true
                };

                foreach (var category in categories)
                {
                    ProductCategory newProductCategory = new ProductCategory();
                    newProductCategory.Product = product;
                    newProductCategory.CategoryId = category.Id;

                    product.ProductCategories.Add(newProductCategory);
                }

                await _productRepository.AddAsync(product);

                await _productRepository.SaveChangesAsync();
                await _productRepository.CommitTransactionAsync();

                _logger.LogInformation($"Product berhasil dibuat: Product {product.Id}, Store {product.StoreId}, Product {product.ProductName}");
                var created = await GetByIdAsync(product.Id);
                return created != null ? ToProductResponse(created) : null;
            }
            catch
            {
                await _productRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<Product>> GetAllAsync(string? search, long? sellerId, long? categoryId, int page, int limit)
        {
            var products = await _productRepository.GetAllAsync("Store", "ProductCategories", "Reviews", "OrderItems");

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

                products = products.OrderByDescending(p => p.Id).ToList();
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

            var pagedProducts = products.Skip((page - 1) * limit).Take(limit).ToList();
            foreach (var p in pagedProducts)
            {
                p.Rating = p.Reviews != null && p.Reviews.Any()
                    ? Math.Round(p.Reviews.Average(r => (double)r.Rating), 1)
                    : 0;
                p.TotalReviews = p.Reviews?.Count ?? 0;
                p.SalesCount = p.OrderItems?.Sum(oi => oi.Quantity) ?? 0;
            }

            return pagedProducts;
        }

        public async Task<Product?> GetByIdAsync(long productId, long? sellerId = null)
        {
            var product = await _productRepository.GetByIdAsync(productId, "ProductCategories.Category", "Store", "Reviews", "OrderItems");

            if (product == null || product.isActive != true) return null;
            if (sellerId.HasValue && product.Store?.UserId != sellerId.Value) return null;

            product.Rating = product.Reviews != null && product.Reviews.Any()
                ? Math.Round(product.Reviews.Average(r => (double)r.Rating), 1)
                : 0;
            product.TotalReviews = product.Reviews?.Count ?? 0;
            product.SalesCount = product.OrderItems?.Sum(oi => oi.Quantity) ?? 0;

            return product;
        }

        public async Task<ProductResponse?> UpdateAsync(long productId, long sellerId, ProductRequest request)
        {
            await _productRepository.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                var product = await GetByIdAsync(productId, sellerId);
                if (product == null || (request.StoreId != 0 && product.StoreId != request.StoreId))
                {
                    await _productRepository.RollbackTransactionAsync();
                    return null;
                }

                if (request.HasExpectedStock && request.ProductStock == null)
                    throw new ValidationException("Sertakan stok baru jika stok awal dikirim.");

                if (request.ProductStock.HasValue && !request.HasExpectedStock)
                    throw new ValidationException("Sertakan expectedStock saat mengubah stok.");

                if (request.HasExpectedStock && request.ExpectedStock != product.ProductStock)
                    throw new DBConcurrencyException("Stok produk telah berubah. Muat ulang produk sebelum menyimpan.");

                if (request.CategoryIds != null)
                {
                    await UpdateCategoriesAsync(product, request.CategoryIds);
                }

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

                product.UpdatedAt = DateTime.UtcNow;
                await _productRepository.SaveChangesAsync();
                await _productRepository.CommitTransactionAsync();
                return ToProductResponse(product);
            }
            catch (Exception ex)
            {
                await _productRepository.RollbackTransactionAsync();
                if (ex is DbUpdateConcurrencyException ||
                    ex.GetBaseException() is PostgresException { SqlState: "40001" or "40P01" })
                {
                    throw new DBConcurrencyException("Produk telah berubah. Muat ulang produk sebelum menyimpan.", ex);
                }
                throw;
            }
        }
        public async Task<bool> DeleteAsync(long productId, long sellerId)
        {
            var product = await GetOwnedProductAsync(productId, sellerId);
            if (product == null) return false;
            if (product.isActive == false) return true;

            product.isActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepository.SaveChangesAsync();
            return true;
        }

        private async Task<Product?> GetOwnedProductAsync(long productId, long sellerId)
        {
            var product = await _productRepository.GetByIdAsync(productId, "Store");
            if (product == null || product.Store?.UserId != sellerId) return null;
            return product;
        }

        private async Task<List<Category>> GetCategoriesAsync(List<long>? categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                throw new ValidationException("Pilih minimal satu kategori produk.");

            var requestedIds = categoryIds.ToHashSet();
            var categories = await _categoryRepository.GetAllAsync();
            var selectedCategories = categories.Where(category => requestedIds.Contains(category.Id)).ToList();
            if (selectedCategories.Count != requestedIds.Count)
                throw new ValidationException("Kategori produk tidak valid.");

            return selectedCategories;
        }

        private async Task UpdateCategoriesAsync(Product product, List<long> categoryIds)
        {
            var categories = await GetCategoriesAsync(categoryIds);
            var newCategoryIds = categories.Select(category => category.Id).ToHashSet();

            foreach (var relation in product.ProductCategories.ToList())
            {
                if (newCategoryIds.Remove(relation.CategoryId)) continue;

                _productCategoryRepository.Delete(relation);
                product.ProductCategories.Remove(relation);
            }

            foreach (var category in categories.Where(category => newCategoryIds.Contains(category.Id)))
            {
                await _productCategoryRepository.AddAsync(new ProductCategory
                {
                    Product = product,
                    Category = category
                });
            }
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
                Rating = product.Rating,
                SalesCount = product.SalesCount,
                TotalReviews = product.TotalReviews,
                UpdatedAt = product.UpdatedAt ?? DateTime.UtcNow,
                CreatedAt = product.CreatedAt ?? DateTime.UtcNow,
            };
        }
    }
}
