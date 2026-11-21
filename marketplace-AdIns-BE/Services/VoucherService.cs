using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace marketplace_AdIns_BE.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IRepository<Store> _storeRepository;

        public VoucherService(IRepository<Voucher> voucherRepository, IRepository<Store> storeRepository)
        {
            _voucherRepository = voucherRepository;
            _storeRepository = storeRepository;
        }

        public async Task<List<VoucherResponse>?> GetByStoreIdAsync(long storeId, long? sellerId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId, "Vouchers");
            if (store == null || (sellerId.HasValue && store.UserId != sellerId)) return null;

            return store.Vouchers.OrderByDescending(v => v.Id).Select(ToVoucherResponse).ToList();
        }

        public async Task<VoucherResponse?> CreateAsync(long storeId, long sellerId, CreateVoucherRequest request)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null || store.UserId != sellerId) return null;

            var now = DateTime.UtcNow;
            var voucher = new Voucher
            {
                StoreId = storeId,
                Code = request.Code.Trim(),
                DiscountPercentage = request.DiscountPercentage,
                MinPurchaseAmount = request.MinPurchaseAmount,
                MaxDiscountAmount = request.MaxDiscountAmount,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _voucherRepository.AddAsync(voucher);
            try
            {
                await _voucherRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_Vouchers_StoreId_Code" or "IX_Vouchers_Code" })
            {
                throw new ConstraintException("Kode voucher sudah digunakan.", ex);
            }
            return ToVoucherResponse(voucher);
        }

        public async Task<VoucherResponse?> UpdateStatusAsync(long storeId, long voucherId, long sellerId, bool isActive)
        {
            var voucher = await GetOwnedVoucherAsync(storeId, voucherId, sellerId);
            if (voucher == null) return null;
            if (voucher.IsActive == isActive) return ToVoucherResponse(voucher);

            voucher.IsActive = isActive;
            voucher.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _voucherRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            return ToVoucherResponse(voucher);
        }

        public async Task<bool> DeleteAsync(long storeId, long voucherId, long sellerId)
        {
            var voucher = await GetOwnedVoucherAsync(storeId, voucherId, sellerId);
            if (voucher == null) return false;

            _voucherRepository.Delete(voucher);
            try
            {
                await _voucherRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                { SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.RestrictViolation,
                  ConstraintName: "FK_Orders_Vouchers_VoucherId" })
            {
                throw new ConstraintException("Voucher sudah digunakan dalam pesanan. Nonaktifkan voucher ini.", ex);
            }
            return true;
        }

        private async Task<Voucher?> GetOwnedVoucherAsync(long storeId, long voucherId, long sellerId)
        {
            var voucher = await _voucherRepository.GetByIdAsync(voucherId, "Store");
            if (voucher == null || voucher.StoreId != storeId || voucher.Store?.UserId != sellerId) return null;
            return voucher;
        }

        public VoucherResponse ToVoucherResponse(Voucher voucher)
        {
            return new VoucherResponse
            {
                Id = voucher.Id,
                StoreId = voucher.StoreId,
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
