using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/stores/{storeId:long}/vouchers")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet]
        public async Task<IActionResult> GetVouchers(long storeId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            var vouchers = await _voucherService.GetByStoreIdAsync(storeId, role == "Seller" ? userId : null);
            if (vouchers == null) return NotFound();
            return Ok(vouchers);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> CreateVoucher(long storeId, [FromBody] CreateVoucherRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            try
            {
                var voucher = await _voucherService.CreateAsync(storeId, userId, request);
                if (voucher == null) return NotFound();
                return Created($"/api/stores/{storeId}/vouchers/{voucher.Id}", voucher);
            }
            catch (ConstraintException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpPatch("{voucherId:long}")]
        public async Task<IActionResult> UpdateVoucherStatus(long storeId, long voucherId, [FromBody] UpdateVoucherStatusRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var voucher = await _voucherService.UpdateStatusAsync(storeId, voucherId, userId, request.IsActive!.Value);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{voucherId:long}")]
        public async Task<IActionResult> DeleteVoucher(long storeId, long voucherId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            try
            {
                var deleted = await _voucherService.DeleteAsync(storeId, voucherId, userId);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (ConstraintException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
