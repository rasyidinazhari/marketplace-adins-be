using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Services
{
    public interface IStoreService
    {
        Task<List<StoreResponse>> GetAllForSellerAsync(long sellerId);
    }
}
