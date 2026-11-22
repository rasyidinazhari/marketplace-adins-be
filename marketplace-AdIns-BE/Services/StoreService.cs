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

        public async Task<StoreResponse> CreateAsync(StoreRequest request)
        {
            await _storeRepository.BeginTransactionAsync();

            try
            {
                var normalizedName = request.StoreName.Trim();

                var store = new Store
                {
                    UserId = request.UserId,
                    StoreName = normalizedName
                };

                await _storeRepository.AddAsync(store);
                await _storeRepository.SaveChangesAsync();
                await _storeRepository.CommitTransactionAsync();

                return new StoreResponse
                {
                    Id = store.Id,
                    StoreName = store.StoreName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                await _storeRepository.RollbackTransactionAsync();
                throw;
            }
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