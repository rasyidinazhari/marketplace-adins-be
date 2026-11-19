using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.Services;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    public class VoucherController : ControllerBase
    {
        /*private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [Authorize(Roles = "Buyer")]
        [HttpGet]
        public async Task<IActionResult> GetVouchers()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return Ok(vouchers);
        }*/
    }
}
