using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;

namespace marketplace_AdIns_BE.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;

        public StoreService(IRepository<Store> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<List<StoreResponse>> GetAllForSellerAsync(long sellerId)
        {
            var stores = await _storeRepository.GetAllAsync();
            return stores.Where(store => store.UserId == sellerId)
                .OrderBy(store => store.Id)
                .Select(store => new StoreResponse
                {
                    Id = store.Id,
                    StoreName = store.StoreName
                }).ToList();
        }
    }
}
