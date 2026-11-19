using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Authorize(Roles = "Buyer")]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var cart = await _cartService.GetAllAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var cart = await _cartService.CreateAsync(userId, request);
            return Ok(cart);
        }

        [HttpPatch("items/{productId:long}")]
        public async Task<IActionResult> UpdateItem(
            long productId, [FromBody] UpdateCartItemRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var cart = await _cartService.UpdateAsync(userId, productId, request.Quantity);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("items/{productId:long}")]
        public async Task<IActionResult> RemoveItem(long productId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            bool removed = await _cartService.DeleteAsync(userId, productId);
            if (!removed) return NotFound();
            return NoContent();
        }
    }
}
