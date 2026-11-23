using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/carts")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartsByUserId()
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
        public async Task<IActionResult> AddItem([FromBody] CartRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            request.UserId = userId;
            var cart = await _cartService.CreateAsync(request);
            return Ok(cart);
        }

        [HttpPut("items/{cartId:long}")]
        public async Task<IActionResult> UpdateCart(
            long cartId, [FromBody] CartRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var cart = await _cartService.UpdateAsync(cartId, request);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("items/{cartId:long}")]
        public async Task<IActionResult> RemoveCartItem(long cartId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            bool removed = await _cartService.DeleteAsync(cartId);
            if (!removed) return NotFound();
            return NoContent();
        }
    }
}
