using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.Services;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/seller/products")]
    public class SellerProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public SellerProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] long categoryId = 0,
            [FromQuery] long? storeId = null)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            if (page <= 0 || limit <= 0 || (long)(page - 1) * limit > int.MaxValue)
            {
                return BadRequest(new { message = "Page atau limit tidak valid." });
            }

            if (storeId is <= 0)
            {
                return BadRequest(new { message = "StoreId harus lebih dari 0." });
            }

            var products = await _productService.GetAllAsync(search, userId, categoryId, page, limit, storeId);
            return Ok(products);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var product = await _productService.GetByIdAsync(id, userId);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}
