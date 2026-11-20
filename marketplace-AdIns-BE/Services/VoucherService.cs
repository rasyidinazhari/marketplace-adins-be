using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;

namespace marketplace_AdIns_BE.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IRepository<Voucher> _voucherRepository;

        public VoucherService(IRepository<Voucher> voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<List<VoucherResponse>> GetAllAsync()
        {
            var vouchers = await _voucherRepository.GetAllAsync();
            return vouchers.Select(ToVoucherResponse).ToList();
        }

        public VoucherResponse ToVoucherResponse(Voucher voucher)
        {
            return new VoucherResponse
            {
                Id = voucher.Id,
                Code = voucher.Code,
                DiscountPercentage = voucher.DiscountPercentage,
                MinPurchaseAmount = voucher.MinPurchaseAmount,
                MaxDiscountAmount = voucher.MaxDiscountAmount,
                IsActive = voucher.IsActive,
                CreatedAt = voucher.CreatedAt,
                UpdatedAt = voucher.UpdatedAt
            };
        }
    }
}
