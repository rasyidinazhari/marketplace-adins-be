using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;

namespace marketplace_AdIns_BE.Services
{
    public class LandingService : ILandingService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Store> _storeRepository;

        public LandingService(IRepository<Product> productRepository, IRepository<Store> storeRepository)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
        }

        public async Task<LandingStatsResponse> GetStatsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var stores = await _storeRepository.GetAllAsync();

            var activeProductsCount = products.Count(p => p.isActive == true);
            var activeSellersCount = stores.Select(s => s.UserId).Distinct().Count();

            return new LandingStatsResponse
            {
                TotalProducts = activeProductsCount,
                TotalSellers = activeSellersCount
            };
        }
    }
}
