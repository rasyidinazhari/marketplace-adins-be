using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface IVoucherService
    {
        Task<List<VoucherResponse>> GetAllAsync();
        VoucherResponse ToVoucherResponse(Voucher voucher);
    }
}
