using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface IVoucherService
    {
        Task<List<VoucherResponse>?> GetByStoreIdAsync(long storeId, long? sellerId);
        Task<VoucherResponse?> CreateAsync(long storeId, long sellerId, CreateVoucherRequest request);
        Task<VoucherResponse?> UpdateStatusAsync(long storeId, long voucherId, long sellerId, bool isActive);
        Task<bool> DeleteAsync(long storeId, long voucherId, long sellerId);
        VoucherResponse ToVoucherResponse(Voucher voucher);
    }
}
