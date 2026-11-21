using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.Services;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/seller/stores")]
    public class SellerStoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public SellerStoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var stores = await _storeService.GetAllForSellerAsync(userId);
            return Ok(stores);
        }
    }
}
